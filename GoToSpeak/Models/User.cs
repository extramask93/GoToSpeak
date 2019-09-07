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
        public string PhotoUrl { get; set; }
        public string PhotoPublicID {get; set;}
        public virtual ICollection<Message> MessagesSent { get; set; }
        public virtual ICollection<Message> MessagesReceived { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set;}
        public string SuccessfullLoginAgent { get;set; }
        public string SuccessfullLoginIp { get; set; }
        public Nullable<DateTime> SuccessfullLoginTimestamp { get; set; }
        public string FailedfullLoginAgent { get;set; }
        public string FailedfullLoginIp { get; set; }
        public Nullable<DateTime> FailedfullLoginTimestamp { get; set; }
    }
}
