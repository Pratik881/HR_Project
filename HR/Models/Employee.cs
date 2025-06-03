namespace HR.Models
{
    public class Employee
    {
        public int  Id { get; set; }
        public string FullName { get; set; }
        
        public string Email { get; set; }   

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; }

        public int LeavePoints { get; set; } = 20;
        public string ApplicationUserId{ get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
