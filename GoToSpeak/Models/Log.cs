using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace GoToSpeak.Models
{
    public class Log {
    public Log()
    {
    }

    public Log(int level, string message)
    {
        Level = level;
        Timestamp = DateTime.Now;
        Message = message;
    }
    [Key]
    public int Id { get; set; }
    public Nullable<int> UserId { get; set; }
    public int Level { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    }
}