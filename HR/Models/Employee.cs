namespace HR.Models
{
    public class Employee
    {
        public int  Id { get; set; }
        //Non-nullable property 'FullName' must contain a non-null value when exiting constructor.
        //public string FullName { get; set; } 
        //string is non-nullable as by default as set in .csproj file.
        //but c# needs guarentee : either assign a value immediately, mark it required, give a default(=""), make nullable string?

        public required string FullName { get; set; }

        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

        public required string Department { get; set; }

        public required string Position { get; set; }
       //value type haru non-nullable by default.C# assigns a default value on its own.
        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; }

        public int LeavePoints { get; set; } = 20;
        public  required string ApplicationUserId{ get; set; }

        public   ApplicationUser ApplicationUser { get; set; }
    }
}
