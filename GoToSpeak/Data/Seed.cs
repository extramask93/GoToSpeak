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
        public void SeedLogs()
        {
            var user = _context.Users.FirstOrDefault();
            if(!_context.Logs.Any()) {
                Log log = new Log { UserId = user.Id,Timestamp = DateTime.Now, Message="User A has been blocked due to too many login attempts", Level=3};
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
        }
        public void SeedRooms()
        {
            if (!_context.Rooms.Any())
            {
                Room room = new Room { Name = "Lobby" };
                //_context.Rooms.Add(room);
                //_context.SaveChanges();
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
                    UserName = "Admin",
                    Email = "jozwiak.damian02@gmail.com",
                    PhotoUrl = "https://res.cloudinary.com/dbxqf9dsq/image/upload/v1560411581/user_ddvo0l.png"
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