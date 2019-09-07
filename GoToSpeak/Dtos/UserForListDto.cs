using System;
using GoToSpeak.Models;

namespace GoToSpeak.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string SuccessfullLoginAgent { get;set; }
        public string SuccessfullLoginIp { get; set; }
        public Nullable<DateTime> SuccessfullLoginTimestamp { get; set; }
        public string FailedfullLoginAgent { get;set; }
        public string FailedfullLoginIp { get; set; }
        public Nullable<DateTime> FailedfullLoginTimestamp { get; set; }
        public string PhotoUrl { get; set; }
        public string CurrentRoom { get; set; }
    }
}