using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection.Metadata;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authServices;

        public AuthController(IAuthService authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await _authServices.RegisterUserAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response.Message); // Return 400 with error message
            }

            return Ok(response); // Return 200 with response (user data and success message)
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response = await _authServices.LoginUserAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response.Message); // Return 400 with error message
            }

            return Ok(response); // Return 200 with response (LoginResponse data and success message)
        }


        [HttpPost("forget-password")]
        public async Task <IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {
            var response= await _authServices.ForgetPasswordServiceAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var response = await _authServices.ResetPasswordAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response.Message); // Return 400 with error message
            }

            return Ok(response); // Return 200 with response (success message)

        }
    }
}
