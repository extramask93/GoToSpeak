using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoToSpeak.Dtos;
using GoToSpeak.Helpers;
using GoToSpeak.Models;
using Microsoft.EntityFrameworkCore;

namespace GoToSpeak.Data
{
    public class RolesRepository : IRolesRepository
    {
        private readonly DataContext context;
        public RolesRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<PagedList<UserForListDto>> GetUsersWithRoles(UserParams userParams)
        {
            var users = context.Users
            .OrderBy(x => x.UserName)
            .Select(user => new UserForListDto
             {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (from userRole in user.UserRoles
                        join role in context.Roles
                        on userRole.RoleId
                        equals role.Id
                        select role.Name).ToList()
            }
            );
            var userList = await PagedList<UserForListDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            return userList;
        }
    }
}