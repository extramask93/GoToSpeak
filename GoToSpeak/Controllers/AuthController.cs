using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
            var result = await _signInManager.CheckPasswordSignInAsync(user,UserForLoginDto.Password,true);
            if (result.IsLockedOut) {
                return Unauthorized(new {message = "Account has been locked"});
            }
            if(result.Succeeded) {
            var token = GenerateJwtToken(user).Result;
            var userToReturn = _mapper.Map<UserForListDto>(user);
            return Ok(new { token = token, user = userToReturn });
            }
            return Unauthorized(new { message = "Username or password is incorrect" });
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
    }
}