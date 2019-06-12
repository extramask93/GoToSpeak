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
        Task<IEnumerable<User>> GetUsers();
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}