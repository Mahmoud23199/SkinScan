using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SkinScan_API.Common;
using SkinScan_API.Dtos;
using SkinScan_BL.Contracts;
using SkinScan_Core.Contexts;
using SkinScan_Core.Entites;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;


namespace SkinScan_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IGenericRepository<Patient> _pationRepository;
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public AuthenticationController(IGenericRepository<Doctor> doctorRepository,IEmailSender emailSender,IGenericRepository<Patient> pationRepository,UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._configuration = configuration;
            this._pationRepository = pationRepository;
            this._emailSender = emailSender;
            this._doctorRepository = doctorRepository;
        }

        /// <summary>
        /// UserType Patient as 0, Doctor  as 1
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input data", 400));
            }

            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Email is already taken", 400));
            }

            var applicationUser = new ApplicationUser
            {
                UserName = userDto.Name,
                Email = userDto.Email,
                PhoneNumber = userDto.Phone,
                UserType = userDto.userType 
            };

            var identityResult = await _userManager.CreateAsync(applicationUser, userDto.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest(ResponseModel<string>.ErrorResponse(identityResult.Errors.First().Description, 400));
            }

            if (userDto.userType == UserType.Patient)
            {
                var patient = new Patient
                {
                    UserId = applicationUser.Id,
                    Age = userDto.Age,
                };
                await _pationRepository.AddedAsync(patient);
            }
            else if (userDto.userType == UserType.Doctor)
            {
                var doctor = new Doctor
                {
                    UserId = applicationUser.Id,
    
                };
                await _doctorRepository.AddedAsync(doctor);
            }

            return Ok(ResponseModel<object>.SuccessResponse(new
            {
                Id = applicationUser.Id,
                Name = applicationUser.UserName,
                Email = applicationUser.Email,
                Phone = applicationUser.PhoneNumber,
                UserType = applicationUser.UserType 
            }, "Account created successfully", 201));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Invalid login credentials", 400));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.password))
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Invalid email or password", 400));
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),

            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Generate JWT Token
            var authSecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:authSecuretKey"]));
            var credentials = new SigningCredentials(authSecretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                expires: DateTime.UtcNow.AddDays(1),
                claims: claims,
                signingCredentials: credentials
            );

            var tokenResponse = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            };

            return Ok(ResponseModel<object>.SuccessResponse(tokenResponse, "Login successful", 200));
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var currentUser = await _userManager.GetUserAsync(User); 
            if(currentUser == null)
                return BadRequest(ResponseModel<string>.ErrorResponse("Invalid login credentials", 400));

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Invalid data provided",
                    ModelState
                ));
            }

            var user = await _userManager.FindByIdAsync(currentUser.Id);
            if (user == null)
            {
                return NotFound(new ResponseModel<string>(
                    StatusCodes.Status404NotFound,
                    false,
                    "User not found",
                    ""
                ));
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new ResponseModel<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Password change failed",
                    result.Errors
                ));
            }

            return Ok(new ResponseModel<object>(
                StatusCodes.Status200OK,
                true,
                "Password changed successfully",
                null
            ));
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Invalid data provided",
                    ModelState
                ));
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new ResponseModel<string>(
                    StatusCodes.Status404NotFound,
                    false,
                    "User not found",
                    ""
                ));
            }
            if (!Utilities.Helper.IsStrongPassword(forgotPasswordDto.NewPassword))
            {
                return BadRequest(ResponseModel<string>.ErrorResponse("Password must be at least 8 characters long, include uppercase, lowercase, digit, and special character.", 400));
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var tokenWithoutSlash = resetToken.Replace("/", "&Slash&");

            string temporaryPassword =forgotPasswordDto.NewPassword ;

            var resetLink = Url.Action(
                nameof(ResetPassword),
                "Authentication",
                new { token = tokenWithoutSlash, email = user.Email, newPassword = temporaryPassword },
                Request.Scheme
            );

             bool isSent = await _emailSender.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>"
            );

            if (!isSent)
            {
                return BadRequest(new { Message = "Failed to send reset email." });
            }

            return Ok(new ResponseModel<string>(
                StatusCodes.Status200OK,
                true,
                "Password reset token generated successfully",
                resetToken 
            ));
        }

        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            if (!ModelState.IsValid)
            {
                var errorList = new StringBuilder("<h3>Invalid data provided</h3><ul>");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    errorList.AppendFormat("<li>{0}</li>", error.ErrorMessage);
                }
                errorList.Append("</ul>");

                return new ContentResult
                {
                    Content = errorList.ToString(),
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            token = token.Replace("&Slash&", "/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var content = "<h3>User not found</h3><p>Please check the entered email address.</p>";
                return new ContentResult
                {
                    Content = content,
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errorList = new StringBuilder("<h3>Password reset failed</h3><ul>");
                foreach (var error in result.Errors)
                {
                    errorList.AppendFormat("<li>{0}</li>", error.Description);
                }
                errorList.Append("</ul>");

                return new ContentResult
                {
                    Content = errorList.ToString(),
                    ContentType = "text/html",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var successContent = "<h3>Password reset successfully</h3><p>You can now log in with your new password.</p>";
            return new ContentResult
            {
                Content = successContent,
                ContentType = "text/html",
                StatusCode = StatusCodes.Status200OK
            };
        }

    }
}
