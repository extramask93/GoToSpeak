using System.Threading.Tasks;
using GoToSpeak.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GoToSpeak.Dtos;
using Microsoft.AspNetCore.Identity;
using GoToSpeak.Models;
using AutoMapper;
using System.Collections.Generic;
using System;

namespace GoToSpeak.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        public DataContext _context { get; set; }
        public UserManager<User> _userManager { get; set; }
        private readonly IMapper _mapper;
        public AdminController(DataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;

        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
                                               on userRole.RoleId
                                               equals role.Id
                                               select role.Name).ToList()
                                  }).ToListAsync();
            return Ok(userList);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles= await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded) 
            {
                return BadRequest("Failed to add to roles");
            }
            result = await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));
            if(!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles");
            }
            return Ok(await _userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery]FilterDto filters) {

            Func<IQueryable<Log>,FilterDto, List<Log>> filterData = (logz, filterz) => {
                var filteredLogz = logz.Include(u => u.User).Where((log) => log.Timestamp >= filterz.MinDate );
                filteredLogz = filteredLogz.Where((log) => log.UserId.Equals(filterz.UserId ?? log.UserId));
                filteredLogz = filteredLogz.Where((log) => log.Level >= filterz.Severity);
                return filteredLogz.ToList();
            };

            var logs = _context.Logs.Include(u => u.User);
            var filteredLogs = filterData(logs,filters);
            var logsToReturn = _mapper.Map<IEnumerable<LogToReturnDto>>(filteredLogs);
            return Ok(logsToReturn);
        }
    }
}