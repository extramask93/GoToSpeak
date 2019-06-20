using System;

namespace GoToSpeak.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime LastActive { get;set; }
        public string PhotoUrl { get; set; }
        public string CurrentRoom { get; set; }
    }
}