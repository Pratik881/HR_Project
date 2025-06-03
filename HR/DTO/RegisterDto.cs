namespace HR.DTO
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        
        public string Email { get; set; }   

        public string PhoneNumber {  get; set; }
        public string Password { get; set; }

        public string Department {  get; set; }

        public string Position { get; set; }

        public DateTime DateOfJoining {  get; set; }

        public decimal Salary { get; set; } 
    }
}
