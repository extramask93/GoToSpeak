using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoToSpeak.Helpers;
using GoToSpeak.Models;
using Microsoft.EntityFrameworkCore;

namespace GoToSpeak.Data
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext _context;

        public ChatRepository(DataContext context)
        {
            this._context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public void Update<T>(T entity) where T:class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        /*public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).Include(u => u.Recipient).AsQueryable();
            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead == false && u.RecipientDeleted == false);
                    break;
            }
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber, messageParams.PageSize);
        }*/

        public async Task<PagedList<Message>> GetMessageThread(int userId, int recipientId,MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).Include(u => u.Recipient)
            .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted==false)
             .OrderByDescending(m => m.MessageSent).AsQueryable();
             var list = await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber, messageParams.PageSize);
             list.Reverse();
             return list;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.AsQueryable();
            var list = await PagedList<User>.CreateAsync(users,userParams.PageNumber, userParams.PageSize);
            return list;    
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Room>> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return rooms;
        }
    }
}