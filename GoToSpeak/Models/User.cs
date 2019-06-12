using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoToSpeak.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime LastActive { get;set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoPublicID {get; set;}
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        
    }
}
