using System;

namespace GoToSpeak.Dtos
{
    public class LogToReturnDto
    {
        public Nullable<int> UserId { get; set; }
        public int Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}