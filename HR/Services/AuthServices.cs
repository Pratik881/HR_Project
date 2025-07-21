using HR.Data;
using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Formats.Asn1;
using System.Net;

namespace HR.Services
{
    public class AuthServices(UserManager<ApplicationUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, IConfiguration configuration, IEmailService emailService) : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _context = context;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailService _emailService = emailService;

        public async Task<ServiceResponse<string>> RegisterUserAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ServiceResponse<string>.Fail("User with this email already exists");
            }
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,

            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return ServiceResponse<string>.Fail(string.Join(";", result.Errors.Select(e => e.Description)));
            }
            await _userManager.AddToRoleAsync(user,dto.Role);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

             var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";
             Console.WriteLine($"[ResetLink] Generated password reset link for {user.Email}: {resetLink}");
              await _emailService.SendEmailAsync(user.Email, "Set your password",
      $"Hello {user.FullName},<br><br>To set your password, please click the link below:<br><a href='{resetLink}'>Reset Password</a>");
            var employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Position = dto.Position,
                DateOfJoining = dto.DateOfJoining,
                Salary = dto.Salary,
                ApplicationUserId = user.Id
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return ServiceResponse<string>.Ok("User registered successfully and employee created.");

        }

        public async Task<ServiceResponse<LoginResponse>> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return ServiceResponse<LoginResponse>.Fail("Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!result)
            {
                return ServiceResponse<LoginResponse>.Fail("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.CreateToken(user, roles);

            var response = new LoginResponse
            {
                Token = token,
                FullName = user.FullName,
                Roles = roles.ToList()
            };

            return ServiceResponse<LoginResponse>.Ok(response, "Login successful");
        }

        public async Task<ServiceResponse<string>> ForgetPasswordServiceAsync(ForgetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                // Return a success response with a generic message as we don't want to reveal whether the email exists or not.
                return ServiceResponse<string>.Ok("If the email exists, a reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //To make token safe for use inside URL query parameters without corruption
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={dto.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(dto.Email, "Password-reset",
                $"Reset your password by clicking here <a href='{resetLink}'> here </a>.");

            return ServiceResponse<string>.Ok("If the email exists, a reset link has been sent.");
        }

        // Refactored ResetPasswordAsync
        public async Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return ServiceResponse<string>.Fail("User not found.");
            }
             var decodedToken = WebUtility.UrlDecode(dto.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (result.Succeeded)
            {
                return ServiceResponse<string>.Ok("Password reset successful.");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ServiceResponse<string>.Fail($"Password reset failed: {errors}");
            }

        }
    }
}


