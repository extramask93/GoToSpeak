using System.Threading.Tasks;
using GoToSpeak.Dtos;
using GoToSpeak.Helpers;

namespace GoToSpeak.Data
{
    public interface IRolesRepository
    {
        Task<PagedList<UserForListDto>> GetUsersWithRoles(UserParams userParams);

    }
}