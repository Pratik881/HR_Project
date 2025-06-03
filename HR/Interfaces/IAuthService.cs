using HR.DTO;
using HR.Models;
using HR.Utilities;

namespace HR.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<ApplicationUser>> RegisterUserAsync(RegisterDto dto);
        Task<ServiceResponse<LoginResponse>> LoginUserAsync(LoginDto dto);
        Task<ServiceResponse<string>> ForgetPasswordServiceAsync(ForgetPasswordDto dto);
        Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
