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
using GoToSpeak.Helpers;
using System.Net.Http;

namespace GoToSpeak.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        public DataContext _context { get; set; }
        public UserManager<User> _userManager { get; set; }
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;
        private readonly IRolesRepository _rolesRepository;

        public AdminController(DataContext context, UserManager<User> userManager, IMapper mapper
        , ILogRepository logRepository, IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _logRepository = logRepository;
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles([FromQuery]UserParams userParams)
        {
            var users = await _rolesRepository.GetUsersWithRoles(userParams);      
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles");
            }
            return Ok(await _userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery]LogParams logParams)
        {

            var logs = await _logRepository.GetLogs(logParams);
            Response.AddPagination(logs.CurrentPage, logs.PageSize, logs.TotalCount, logs.TotalPages);
            return Ok(logs);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("clearlogs")]
        public IActionResult ClearLogs(HttpRequestMessage request)
        {
            try {
            _logRepository.ClearLogs();
            }
            catch(Exception e) {
                return BadRequest(e.ToString());
            }
            return Ok(new {message = "done"});
        }
    }
}