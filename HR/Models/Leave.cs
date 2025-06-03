using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Models
{
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum LeaveType
    {
        Paid,
        Unpaid
    }
    public class Leave
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate {  get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Reason {  get; set; }

        public LeaveStatus Status { get; set; }=LeaveStatus.Pending;

        public LeaveType Type { get; set; }

        public DateTime RequestedAt { get; set; }=DateTime.Now;

        public int PaidLeaveDays {  get; set; }

        public int UnpaidLeaveDays { get; set;}

        [Required]
        public int ApplicationUserId {  get; set; }
        [ForeignKey("ApplicationUserId ")]
        public Employee Employee {  get; set; }
    }
}
