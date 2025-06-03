using HR.Models;
using Microsoft.Identity.Client;

namespace HR.DTO
{
    public class LeaveSummaryDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }    
        public string Email { get; set; }
        public int TotalLeaveRequests {  get; set; }
        public int PaidLeaveDays {  get; set; }
        public int UnpaidLeaveDays { get; set; }

        public int LeavesTakenThisYear {  get; set; }

        public int LeavePoints {  get; set; }
        
        public string LeaveStatus { get; set; }
    }
}
