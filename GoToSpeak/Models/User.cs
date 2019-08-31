using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GoToSpeak.Models
{
    public class User : IdentityUser<int>
    {
    
        public string RefreshToken { get; set; }
        public DateTime LastActive { get;set; }
        public string PhotoUrl { get; set; }
        public string PhotoPublicID {get; set;}
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public ICollection<UserRole> UserRoles { get; set;}
        public ICollection<Log> Logs { get; set; }
    }
}
