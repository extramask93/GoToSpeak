using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoToSpeak.Models
{
    public class Log {
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    public int Level { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    }
}