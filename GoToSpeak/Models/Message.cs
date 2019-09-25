using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoToSpeak.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }  
        public User Sender { get; set; }
        public int? RecipientId { get; set; }
        [ForeignKey("RecipientId")]
        public virtual User Recipient { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime? MessageSent { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public int? ToRoomId {get; set;}
        [ForeignKey("ToRoomId")]
        public virtual Room ToRoom { get; set; }

    }
}