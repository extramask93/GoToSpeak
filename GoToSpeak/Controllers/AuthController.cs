using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Google.Authenticator;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GoToSpeak.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public UserManager<User> _userManager { get; }
        public SignInManager<User> _signInManager { get; }
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration config, IMapper mapper, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }
        [HttpPost("emailReset")]
        public async Task<IActionResult> SendPasswordResetLink(UserNameDto userName)
        {
            var user = await _userManager.FindByNameAsync(userName.UserName);
            if (user == null || user.Email == null)
            {
                return BadRequest("User does not exists or lacks email address");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = "https://localhost:4200/resetPassword?token=" + HttpUtility.UrlEncode(token);
            SendEmail(user.Email, "Here is your reset link: " + resetLink);
            return Ok(new { message = "Reset password link has been sent to your email" });
        }
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(PasswordForResetDto passwordForReset)
        {
            if (passwordForReset.Password != passwordForReset.ConfirmPassword)
            {
                return BadRequest("Passwords must be the same");
            }
            var user = await _userManager.FindByNameAsync(passwordForReset.UserName);
            var result = await _userManager.ResetPasswordAsync(user, passwordForReset.Token, passwordForReset.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password has been changed" });
            }
            return BadRequest();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            userToCreate.PhotoUrl = "https://res.cloudinary.com/dbxqf9dsq/image/upload/v1560411581/user_ddvo0l.png";
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            if (result.Succeeded)
            {
                _logger.LogWarning("User {userId} was registered", userForRegisterDto.Username);
                return Ok(new { username = userToCreate.UserName });
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(UserForLoginDto.Username);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            if (user.TwoFactorEnabled)
            {
                return BadRequest(new {message="User requires 2 factor authorization",
                code=101});
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, UserForLoginDto.Password, true);
            if (result.IsLockedOut)
            {
                return BadRequest(string.Format("Account has been locked for 5 minutes due to multiple failed login attemts"));
            }
            if (result.Succeeded)
            {
                user.RefreshToken = GenerateRefreshToken();
                var token = GenerateJwtToken(user).Result;
                var userToReturn = _mapper.Map<UserForListDto>(user);
                _logger.LogWarning("User with id={userId} has logged in", userToReturn.Id);
                var updateResult = await _userManager.UpdateAsync(user);
                return Ok(new { token = token, user = userToReturn, refreshToken = user.RefreshToken });
            }
            int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
            int attemptsLeft = 3 -
                        accessFailedCount;
            return BadRequest(string.Format("Username or password is incorrect, there are {0} tries left before a lockout", attemptsLeft.ToString()));
        }
        [HttpPost("login2fa")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return BadRequest($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.TwoFactorCode);
            if(!result) {
                await _userManager.AccessFailedAsync(user);
                int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                int attemptsLeft = 3 -
                        accessFailedCount;
                return BadRequest(string.Format(
                    "Username or PIN code is incorrect, there are {0} tries left before a lockout", attemptsLeft.ToString()));
            }
            return Ok();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeDto model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return Ok();
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return BadRequest();
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return BadRequest();
            }
        }
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(TokenForRefreshDto tokenForRefresh)
        {
            var principal = GetPrincipalFromExpiredToken(tokenForRefresh.Token);
            var username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var savedRefreshToken = user.RefreshToken; //retrieve the refresh token from a data store
            if (savedRefreshToken != tokenForRefresh.RefreshToken)
                throw new SecurityTokenException("Invalid refresh token");
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        private void SendEmail(string to, string message)
        {
            var client = new SendGridClient("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");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("support@gotospeak.com", "GoToSpeak Team"),
                Subject = "Password reset link.",
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(to));
            var response = client.SendEmailAsync(msg).Result;
        }
    }
}