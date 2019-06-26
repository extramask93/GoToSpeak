using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public UserManager<User> _UserManager { get; }
        public SignInManager<User> _signInManager { get; }

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration config, IMapper mapper)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            this._config = config;
            this._mapper = mapper;
        }
        [HttpPost("emailReset")]
        public async Task<IActionResult> SendPasswordResetLink(UserNameDto userName)
        {
            var user = await _UserManager.FindByNameAsync(userName.UserName);
            if(user == null || user.Email == null)
            {
                return BadRequest("User does not exists or lacks email address");
            }
            var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = "https://localhost:4200/resetPassword?token=" + HttpUtility.UrlEncode(token);
            SendEmail(user.Email,"Here is your reset link: " + resetLink);
            return Ok(new {message = "Reset password link has been sent to your email"});
        }
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(PasswordForResetDto passwordForReset)
        {
            if(passwordForReset.Password != passwordForReset.ConfirmPassword) 
            {
                return BadRequest("Passwords must be the same");
            }
            var user = await _UserManager.FindByNameAsync(passwordForReset.UserName);
            var result = await _UserManager.ResetPasswordAsync(user, passwordForReset.Token, passwordForReset.Password);
            if(result.Succeeded)
            {
                return Ok(new {message= "Password has been changed"});
            }
            return BadRequest();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            userToCreate.PhotoUrl = "https://res.cloudinary.com/dbxqf9dsq/image/upload/v1560411581/user_ddvo0l.png";
            var result = await _UserManager.CreateAsync(userToCreate,userForRegisterDto.Password);
            if(result.Succeeded) {
                return Ok(new { username = userToCreate.UserName });
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto)
        {
            var user = await _UserManager.FindByNameAsync(UserForLoginDto.Username);
            if(user == null)
            {
                return BadRequest("User does not exist");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user,UserForLoginDto.Password,true);
            if (result.IsLockedOut) {
                return BadRequest(string.Format("Account has been locked for 5 minutes due to multiple failed login attemts"));
            }
            if(result.Succeeded) {
            var token = GenerateJwtToken(user).Result;
            var userToReturn = _mapper.Map<UserForListDto>(user);
            return Ok(new { token = token, user = userToReturn });
            }
            int accessFailedCount = await _UserManager.GetAccessFailedCountAsync(user);
            int attemptsLeft = 3-
                        accessFailedCount;
            return BadRequest(string.Format("Username or password is incorrect, there are {0} tries left before a lockout", attemptsLeft.ToString()));
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roles = await _UserManager.GetRolesAsync(user);
            foreach(var role in  roles) {
                claims.Add(new Claim(ClaimTypes.Role,role));
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