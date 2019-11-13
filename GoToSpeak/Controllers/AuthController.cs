using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
using GoToSpeak.Helpers;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using UAParser;

namespace GoToSpeak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public UserManager<User> _userManager { get; }
        public SignInManager<User> _signInManager { get; }
        private readonly IConfiguration _configuration;
        private readonly IDbLogger _logger;
        private IHttpContextAccessor _accessor;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ILogRepository logRepository, DataContext context, 
        IConfiguration config, IMapper mapper, IConfiguration configuration, IDbLogger logger, IHttpContextAccessor accessor)
        {
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _logRepository = logRepository;
            _context = context;
            _accessor = accessor;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("resetLogged")]
        public async Task<IActionResult> ResetPasswordWhileLoggedIn(PasswordForResetDto passwordForReset)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if(passwordForReset.Password != passwordForReset.ConfirmPassword)
            {
                return BadRequest("Passwords must be the same");
            }
            if(await _userManager.CheckPasswordAsync(user, passwordForReset.OldPassword) == false)
            {
                return BadRequest("Old password does not match");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passwordForReset.Password);
            if(result.Succeeded)
            {
                _logger.LogInfo(user.Id,$"Passwor has been changed for user: {user.UserName}");
                return Ok(new { message = "Password has been changed" });
            }
            _logger.LogWarning(user.Id,$"Error occured during passoword reset for user: {user.UserName}");
            return BadRequest("Something went wrong");
        }
        [AllowAnonymous]
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

            var result = SendEmail(user.Email, "Here is your reset link: " + resetLink);
            if(result.StatusCode == HttpStatusCode.Accepted) {
                _logger.LogInfo(user.Id, $"Password reset link has been sent to user: {userName}");
                return Ok(new { message = "Reset password link has been sent to your email" });
            }
            _logger.LogWarning(user.Id, $"Error occured during reset email sending process for user: {userName}");
            return BadRequest("Error occured during mail sending process");
            
        }
        [AllowAnonymous]
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
                _logger.LogInfo(user.Id,$"Passwor has been changed for user: {user.UserName}");
                return Ok(new { message = "Password has been changed" });
            }
            _logger.LogWarning(user.Id, $"Password reset failed for user {user.UserName}");
            return BadRequest("Reset password failed");
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            userToCreate.PhotoUrl = "https://res.cloudinary.com/dbxqf9dsq/image/upload/v1560411581/user_ddvo0l.png";
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            if (result.Succeeded)
            {
                _logger.LogWarning($"New user has been registered, with name: {userForRegisterDto.Username}");
                return Ok(new { username = userToCreate.UserName });
            }
            return BadRequest(result.Errors);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto)
        {
            
            var user = await _userManager.FindByNameAsync(UserForLoginDto.Username);
            if (user == null)
            {         
                _logger.LogWarning($"User with non-existing username: {UserForLoginDto.Username} tried to log in");
                return BadRequest("User does not exist");
            }
            if (user.TwoFactorEnabled)
            {
                return BadRequest("User requires 2 factor authorization");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, UserForLoginDto.Password, true);
            if (result.IsLockedOut)
            {
                _logger.LogWarning(user.Id,$"Temporarly blocked user: {UserForLoginDto.Username} tried to log in");
                return BadRequest(string.Format("Account has been locked for 5 minutes due to multiple failed login attemts"));
            } 
            if (result.Succeeded)
            {
                user.RefreshToken = GenerateRefreshToken();
                var token = GenerateJwtToken(user).Result;
                var userToReturn = _mapper.Map<UserForListDto>(user);
                ClientInfo c = Parser.GetDefault().Parse(Convert.ToString(Request.Headers["User-Agent"][0]));        
                user.SuccessfullLoginTimestamp =  DateTime.UtcNow;
                user.SuccessfullLoginIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                user.SuccessfullLoginAgent = c.ToString();
                var updateResult = await _userManager.UpdateAsync(user);
                _logger.LogInfo(user.Id, $"User {user.UserName} has logged in");
                return Ok(new { token = token, user = userToReturn, refreshToken = user.RefreshToken});
            }
            int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
            int attemptsLeft = 3 -
                    accessFailedCount;
            ClientInfo c2 = Parser.GetDefault().Parse(Convert.ToString(Request.Headers["User-Agent"][0]));        
            user.FailedfullLoginTimestamp =  DateTime.UtcNow;
            user.FailedfullLoginIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            user.FailedfullLoginAgent = c2.ToString();
            await _userManager.UpdateAsync(user);
            _logger.LogWarning(user.Id,$"User: {UserForLoginDto.Username} tried to log in for the {accessFailedCount} time");
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
                return BadRequest("User does not exist");
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
            var result2 = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (result2.IsLockedOut)
            {
                return BadRequest(string.Format("Account has been locked for 5 minutes due to multiple failed login attemts"));
            }
            if (result2.Succeeded)
            {
                user.RefreshToken = GenerateRefreshToken();
                var token = GenerateJwtToken(user).Result;
                var userToReturn = _mapper.Map<UserForListDto>(user);
                var updateResult = await _userManager.UpdateAsync(user);
                return Ok(new { token = token, user = userToReturn, refreshToken = user.RefreshToken });
            }
            int accessFailedCount2 = await _userManager.GetAccessFailedCountAsync(user);
            int attemptsLeft2 = 3 -
                        accessFailedCount2;
            return BadRequest(string.Format("Username or password is incorrect, there are {0} tries left before a lockout", attemptsLeft2.ToString()));
        }
        [HttpPost("loginwithrecoverycode")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeDto model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                user.RefreshToken = GenerateRefreshToken();
                var token = GenerateJwtToken(user).Result;
                var userToReturn = _mapper.Map<UserForListDto>(user);
                return Ok(new { token = token, user = userToReturn, refreshToken = user.RefreshToken });
            }
            if (result.IsLockedOut)
            {
                return BadRequest();
            }
            else
            {
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
                Expires = DateTime.Now.AddMinutes(10),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        private  Response SendEmail(string to, string message)
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
            return response;
        }
    }
}