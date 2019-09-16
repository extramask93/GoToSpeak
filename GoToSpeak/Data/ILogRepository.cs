using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoToSpeak.Helpers;
using GoToSpeak.Models;

namespace GoToSpeak.Data
{
   public interface ILogRepository
    {
        void Add<T>(T entity) where T: class;
        Task<PagedList<Log>> GetLogs(LogParams logParams);
    }
}