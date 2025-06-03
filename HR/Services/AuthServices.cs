using HR.Data;
using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Formats.Asn1;

namespace HR.Services
{
    public class AuthServices:IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthServices(UserManager<ApplicationUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
            _emailService = emailService;
        }


        public async Task<ServiceResponse<ApplicationUser>> RegisterUserAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.FullName,

            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return ServiceResponse<ApplicationUser>.Fail(string.Join(";", result.Errors.Select(e => e.Description)));
            }
            await _userManager.AddToRoleAsync(user, "Employee");

            var employee = new Employee
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password = dto.Password,
                Department = dto.Department,
                Position = dto.Position,
                DateOfJoining = dto.DateOfJoining,
                Salary = dto.Salary,
                ApplicationUserId = user.Id
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(dto.Email, "Welcome to HR System",
                $@"
    <h2>Welcome {dto.FullName}!</h2>
    <p>Your account has been created.</p>
    <p><b>Username:</b> {dto.Email}</p>
    <p><b>Temporary Password:</b> {dto.Password}</p>
    <p>Please login and change your password.</p>
    <p>Login Link: https://yourapp.com/login</p>
");
            return ServiceResponse<ApplicationUser>.Ok(user, "User registered successfully and employee created.");


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

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
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


