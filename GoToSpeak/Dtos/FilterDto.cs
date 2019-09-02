using System;

namespace GoToSpeak.Dtos
{
    public class FilterDto
    {
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }
}