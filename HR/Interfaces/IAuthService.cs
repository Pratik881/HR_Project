using HR.DTO;
using HR.Models;
using HR.Utilities;

namespace HR.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> RegisterUserAsync(RegisterDto dto);
        Task<ServiceResponse<LoginResponse>> LoginUserAsync(LoginDto dto);
        Task<ServiceResponse<string>> ForgetPasswordServiceAsync(ForgetPasswordDto dto);
        Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
