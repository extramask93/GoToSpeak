using System;

namespace GoToSpeak.Dtos
{
    public class LogToReturnDto
    {
        public string User { get; set; }
        public int Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}