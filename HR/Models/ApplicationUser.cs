using Microsoft.AspNetCore.Identity;

namespace HR.Models
{
    public class ApplicationUser:IdentityUser
    {
       // public  required string PhoneNumber{ get; set; } method hiding vayo yo chai
        public  required string FullName { get; set; }
    }
}
