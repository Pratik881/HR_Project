using HR.DTO;
using HR.Models;
using HR.Utilities;

namespace HR.Interfaces
{
    public interface ILeaveService
    {
        Task<ServiceResponse<string>> ApplyForLeave(int employeeId, DateTime startDate, DateTime endDate, string reason);

        Task<ServiceResponse<List<LeaveRequestDto>>> ViewPendingLeaveRequestsAsync();
        

            Task<ServiceResponse<string>> UpdateLeaveRequest(int leaveId, LeaveStatus newStatus);

        Task<ServiceResponse<List<LeaveSummaryDto>>> GetMyLeaves(string applicationUserId);

        Task<ServiceResponse<LeaveSummaryDto>> GetLeaveSummaryAsync(string applicationUserId);

        Task<ServiceResponse<List<DepartmentLeaveSummaryDTO>>> GetDepartmentLeaveSummariesAsync(DateTime queryDate);
    }
}
