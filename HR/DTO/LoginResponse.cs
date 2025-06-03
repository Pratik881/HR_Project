namespace HR.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string  FullName { get; set; }   

        public List<string> Roles { get; set; }
    }
}
