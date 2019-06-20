using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GoToSpeak.Models;

namespace GoToSpeak
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Creator { get; set; }
        public int CreatorId { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}