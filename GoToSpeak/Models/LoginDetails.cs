using System;

namespace GoToSpeak.Models
{
    public class LoginDetailsFailed : LoginDetailsBase {}
    public class LoginDetailsSuccess : LoginDetailsBase {}
    public class LoginDetailsBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime LoginTimestamp { get; set; }
        public string IpAddress { get; set; }
        public string ClientAgent { get; set; }
    }
}