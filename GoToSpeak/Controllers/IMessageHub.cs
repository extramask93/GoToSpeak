using System.Threading.Tasks;
using GoToSpeak.Dtos;

namespace GoToSpeak.Controllers
{
    public interface IMessageHub
    {
         Task AddUser(UserForListDto user);
         Task RemoveUser(UserForListDto user);
         Task OnError(string message);
         Task OnRoomDeleted(string message);
         Task AddChatRoom(RoomToReturn room);
         Task RemoveChatRoom(RoomToReturn room);
         Task NewMessage(MessageToReturnDto message);
         Task NewGlobalMessage(MessageToReturnDto message);
    }
}