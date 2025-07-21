namespace HR.DTO
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        
        public required string Email { get; set; }   

        public  required string PhoneNumber {  get; set; }
        public required string Password { get; set; }

        public required string Department {  get; set; }

        public required string Position { get; set; }

        public string Role { get; set; } = "Employee";

        public DateTime DateOfJoining { get; set; }

        public decimal Salary { get; set; } 
    }
}
