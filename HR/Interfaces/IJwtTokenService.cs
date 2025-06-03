using HR.Models;

namespace HR.Interfaces
{
    public interface IJwtTokenService
    {
       string  CreateToken(ApplicationUser user, IList<string> roles);
    }
}
