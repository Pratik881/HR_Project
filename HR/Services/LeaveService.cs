using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.UoW;
using HR.Utilities;

namespace HR.Services
{
    public class LeaveService(IUnitOfWork unitOfWork) : ILeaveService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
       
        public async Task<ServiceResponse<string>> ApplyForLeave(int employeeId, DateTime startDate, DateTime endDate, string reason)
        {
            var response = new ServiceResponse<string>();

            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
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

            await _unitOfWork.Leaves.AddAsync(leave);
            await _unitOfWork.SaveChangesAsync();

            response.Data = $"Leave request submitted. Paid Days: {paidDays}, Unpaid Days: {unpaidDays}";
            return response;
        }

        public async Task<ServiceResponse<List<LeaveRequestDto>>> ViewPendingLeaveRequestsAsync()
        {
            var pendingLeavesEntities = await _unitOfWork.Leaves.GetLeavesByStatusAsync(LeaveStatus.Pending);

            var pendingLeaves = pendingLeavesEntities
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
                    EmployeeName = l.Employee?.FullName ?? ""
                })
                .ToList();

            return ServiceResponse<List<LeaveRequestDto>>.Ok(pendingLeaves, "Fetched pending leave requests.");
        }

        public async Task<ServiceResponse<string>> UpdateLeaveRequest(int leaveId, LeaveStatus newStatus)
        {
            var response = new ServiceResponse<string>();

            var leave = await _unitOfWork.Leaves.GetByIdAsync(leaveId);
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
                var employee = await _unitOfWork.Employees.GetByIdAsync(leave.ApplicationUserId);
                if (employee != null)
                {
                    int numberOfDays = (leave.EndDate - leave.StartDate).Days + 1;
                    employee.LeavePoints += numberOfDays;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            response.Data = $"Leave has been {newStatus.ToString().ToLower()} successfully.";
            return response;
        }

        public async Task<ServiceResponse<List<LeaveSummaryDto>>> GetMyLeaves(string applicationUserId)
        {
            var response = new ServiceResponse<List<LeaveSummaryDto>>();

            var employee = await _unitOfWork.Employees.GetByApplicationUserIdAsync(applicationUserId);
            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found.";
                return response;
            }

            var leaves = await _unitOfWork.Leaves.GetLeavesByEmployeeIdAsync(employee.Id);

            var leaveSummaries = leaves
                .OrderByDescending(l => l.StartDate)
                .Select(l => new LeaveSummaryDto
                {
                    PaidLeaveDays = l.PaidLeaveDays,
                    UnpaidLeaveDays = l.UnpaidLeaveDays,
                    LeaveStatus = l.Status.ToString(),
                    LeavesTakenThisYear = l.StartDate.Year == DateTime.Now.Year ? 1 : 0,
                })
                .ToList();

            response.Data = leaveSummaries;
            response.Message = leaveSummaries.Count > 0 ? "Leave history retrieved." : "No leave records found.";
            return response;
        }

        public async Task<ServiceResponse<LeaveSummaryDto>> GetLeaveSummaryAsync(string applicationUserId)
        {
            var response = new ServiceResponse<LeaveSummaryDto>();

            var employee = await _unitOfWork.Employees.GetByApplicationUserIdAsync(applicationUserId);
            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                return response;
            }

            var leaves = await _unitOfWork.Leaves.GetLeavesByEmployeeIdAsync(employee.Id);
            var currentYear = DateTime.Now.Year;

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

            var employees = await _unitOfWork.Employees.GetAllAsync();

            var leaves = await _unitOfWork.Leaves.GetApprovedLeavesForDateAsync(queryDate, LeaveStatus.Approved);

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
