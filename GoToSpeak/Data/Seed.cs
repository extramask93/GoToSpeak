using System;
using System.Collections.Generic;
using System.Linq;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace GoToSpeak.Data
{
    public class Seed
    {
        public UserManager<User> _UserManager { get; }
        public DataContext _context { get; }
        public RoleManager<Role> _roleManager { get; }

        public Seed(UserManager<User> userManager, RoleManager<Role> RoleManager, DataContext _context)
        {
            _roleManager = RoleManager;
            this._context = _context;
            _UserManager = userManager;
        }
        public void SeedRooms()
        {
            if (!_context.Rooms.Any())
            {
                Room room = new Room { Name = "Lobby" };
                _context.Rooms.Add(room);
            }
        }
        public void SeedUsers()
        {
            if (!_UserManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role> {
                    new Role{Name = "Admin"},
                    new Role{Name = "User"},
                    new Role{Name = "Moderator"}
                };
                foreach(var role in roles) {
                    _roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users)
                {
                    _UserManager.CreateAsync(user, "password").Wait();
                    _UserManager.AddToRoleAsync(user,"User").Wait();
                }
                var adminUser = new User
                {
                    UserName = "Admin"
                };
                IdentityResult result = _UserManager.CreateAsync(adminUser,"password").Result;
                if(result.Succeeded) {
                    var admin = _UserManager.FindByNameAsync("Admin").Result;
                    _UserManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}).Wait();
                }
            }
        }
    }
}