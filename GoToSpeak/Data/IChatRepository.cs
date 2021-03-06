using System.Collections.Generic;
using System.Threading.Tasks;
using GoToSpeak.Helpers;
using GoToSpeak.Models;

namespace GoToSpeak.Data
{
    public interface IChatRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T:class;
        void Update<T>(T entity) where T:class;
        Task<bool> SaveAll();

        Task<User> GetUser(int id);
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<PagedList<Room>> GetRooms(RoomParams param);
        Task<PagedList<Message>> GetRoomHistory(string roomName, MessageParams param);
        Task<Message> GetMessage(int id);
        //Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<PagedList<Message>> GetMessageThread(int userId, int recipientId, MessageParams messageParams);
    }
}