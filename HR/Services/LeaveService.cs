using Microsoft.EntityFrameworkCore;
using HR.Data;
using HR.Models;
using HR.DTO;
using HR.Utilities;
using HR.Interfaces;
namespace HR.Services
{
    public class LeaveService: ILeaveService
    {
        private readonly AppDbContext _context;

        public LeaveService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> ApplyForLeave(int employeeId, DateTime startDate, DateTime endDate, string reason)
        {
            var response = new ServiceResponse<string>();

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                return response;
            }

            if (startDate > endDate)
            {
                response.Success = false;
                response.Message = "Start date can't be after end date";
                return response;
            }

            int numberOfDays = (endDate - startDate).Days + 1;
            int paidDays = 0;
            int unpaidDays = 0;

            if (employee.LeavePoints >= numberOfDays)
            {
                paidDays = numberOfDays;
                employee.LeavePoints -= numberOfDays;
            }
            else
            {
                paidDays = employee.LeavePoints;
                unpaidDays = numberOfDays - employee.LeavePoints;
                employee.LeavePoints = 0;
            }

            var leave = new Leave
            {
                StartDate = startDate,
                EndDate = endDate,
                Reason = reason,
                ApplicationUserId = employeeId,
                Type = unpaidDays > 0 ? LeaveType.Unpaid : LeaveType.Paid,
                UnpaidLeaveDays = unpaidDays,
                PaidLeaveDays = paidDays,
                Status = LeaveStatus.Pending,
                RequestedAt = DateTime.Now
            };

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();

            response.Data = $"Leave request submitted. Paid Days: {paidDays}, Unpaid Days: {unpaidDays}";
            return response;
        }
        public async Task<ServiceResponse<List<LeaveRequestDto>>> ViewPendingLeaveRequestsAsync()
        {
            
            var pendingLeaves = await _context.Leaves
        .Where(l => l.Status == LeaveStatus.Pending)
        .Include(l => l.Employee) // Include navigation property
        .Select(l => new LeaveRequestDto
        {
            Id = l.Id,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            Reason = l.Reason,
            RequestedAt = l.RequestedAt,
            PaidLeaveDays = l.PaidLeaveDays,
            UnpaidLeaveDays = l.UnpaidLeaveDays,
            EmployeeId = l.ApplicationUserId,
            EmployeeName = l.Employee.FullName
        })
        .ToListAsync();
            return ServiceResponse<List<LeaveRequestDto>>.Ok(pendingLeaves, "Fetched pending leave requests.");
        }

        public async Task<ServiceResponse<string>> UpdateLeaveRequest(int leaveId, LeaveStatus newStatus)
        {
            var response = new ServiceResponse<string>();

            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                response.Success = false;
                response.Message = "Leave not found";
                return response;
            }

            if (leave.Status != LeaveStatus.Pending)
            {
                response.Success = false;
                response.Message = "Leave already processed";
                return response;
            }

            leave.Status = newStatus;

            if (newStatus == LeaveStatus.Rejected)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == leave.ApplicationUserId);
                if (employee != null)
                {
                    int numberOfDays = (leave.EndDate - leave.StartDate).Days + 1;
                    employee.LeavePoints += numberOfDays;
                }
            }

            await _context.SaveChangesAsync();
            response.Data = $"Leave has been {newStatus.ToString().ToLower()} successfully.";
            return response;
        }

        public async Task<ServiceResponse<List<LeaveSummaryDto>>> GetMyLeaves(string applicationUserId)
        {
            var response = new ServiceResponse<List<LeaveSummaryDto>>();

            var employeeId = await _context.Employees
                .Where(e => e.ApplicationUserId == applicationUserId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (employeeId == 0)
            {
                response.Success = false;
                response.Message = "Employee not found.";
                return response;
            }

            var leaves = await _context.Leaves
                .Where(l => l.ApplicationUserId == employeeId)
                .OrderByDescending(l => l.StartDate)
                .Select(l => new LeaveSummaryDto
                {
                    PaidLeaveDays = l.PaidLeaveDays,
                    UnpaidLeaveDays = l.UnpaidLeaveDays,
                    LeaveStatus = l.Status.ToString(),
                    LeavesTakenThisYear = l.StartDate.Year == DateTime.Now.Year ? 1 : 0,
                    
                })
                .ToListAsync();

            response.Data = leaves;
            response.Message = leaves.Count > 0 ? "Leave history retrieved." : "No leave records found.";
            return response;
        }

        public async Task<ServiceResponse<LeaveSummaryDto>> GetLeaveSummaryAsync(string applicationUserId)
        {
            var response = new ServiceResponse<LeaveSummaryDto>();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);

            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                return response;
            }

            var currentYear = DateTime.Now.Year;

            var leaves = await _context.Leaves
                .Where(l => l.ApplicationUserId == employee.Id)
                .ToListAsync();

            response.Data = new LeaveSummaryDto
            {
                EmployeeId = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                TotalLeaveRequests = leaves.Count,
                PaidLeaveDays = leaves.Sum(l => l.PaidLeaveDays),
                UnpaidLeaveDays = leaves.Sum(l => l.UnpaidLeaveDays),
                LeavesTakenThisYear = leaves.Count(l => l.StartDate.Year == currentYear),
                LeavePoints = employee.LeavePoints,
                LeaveStatus = "Summary"
            };

            return response;
        }


        public async Task<ServiceResponse<List<DepartmentLeaveSummaryDTO>>> GetDepartmentLeaveSummariesAsync(DateTime queryDate)
        {
            var response = new ServiceResponse<List<DepartmentLeaveSummaryDTO>>();

            // Get all employees with their leaves and ApplicationUser data
            var employees = await _context.Employees
                .Include(e => e.ApplicationUser)
                .ToListAsync();

            // Get all approved leaves for the given date
            var leaves = await _context.Leaves
                .Where(l => l.Status == LeaveStatus.Approved &&
                            l.StartDate <= queryDate && l.EndDate >= queryDate)
                .ToListAsync();

            // Group employees by department
            var departmentGroups = employees
                .GroupBy(e => e.Department)
                .Select(g =>
                {
                    var deptLeaves = leaves
                        .Where(l => g.Any(e => e.Id == l.ApplicationUserId))
                        .ToList();

                    var employeesOnLeave = deptLeaves
                        .Select(l => g.FirstOrDefault(e => e.Id == l.ApplicationUserId)?.FullName)
                        .Where(name => name != null)
                        .Distinct()
                        .ToList();

                    // Check for conflict: overlapping leaves
                    bool hasConflict = false;

                    for (int i = 0; i < deptLeaves.Count; i++)
                    {
                        for (int j = i + 1; j < deptLeaves.Count; j++)
                        {
                            var l1 = deptLeaves[i];
                            var l2 = deptLeaves[j];

                            if (l1.ApplicationUserId != l2.ApplicationUserId &&
                                l1.StartDate <= l2.EndDate &&
                                l2.StartDate <= l1.EndDate)
                            {
                                hasConflict = true;
                                break;
                            }
                        }

                        if (hasConflict) break;
                    }

                    return new DepartmentLeaveSummaryDTO
                    {
                        Department = g.Key,
                        TotalLeaves = deptLeaves.Count,
                        EmployeesOnLeave = employeesOnLeave,
                        HasConflict = hasConflict
                    };
                })
                .ToList();

            response.Data = departmentGroups;
            response.Success = true;
            return response;
        }




    }

}
