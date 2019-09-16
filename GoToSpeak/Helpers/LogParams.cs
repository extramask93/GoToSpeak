using System;

namespace GoToSpeak.Helpers
{
    public class LogParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; }
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize)? MaxPageSize: value;}
        }
        public int LastXDays { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }
}