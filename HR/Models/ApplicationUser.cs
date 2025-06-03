using Microsoft.AspNetCore.Identity;

namespace HR.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? PhoneNumber{get; set;}
        public string FullName { get; set; }
    }
}
