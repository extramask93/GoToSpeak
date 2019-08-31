using System;

namespace GoToSpeak.Dtos
{
    public class FilterDto
    {
        public string Term { get; set; }
        public DateTime MinDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public int Severity { get; set; }
    }
}