using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoToSpeak.Models;

namespace GoToSpeak.Data
{
   public interface ILogRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T:class;
        void Update<T>(T entity) where T:class;
        Task<bool> SaveAll();   
        Task<IEnumerable<Log>> GetAllLogs();
        Task<IEnumerable<Log>> GetLogsBySeverity(int severity);
        Task<IEnumerable<Log>> GetLogsByMinDate(DateTime date);
        Task<IEnumerable<Log>> GetLogsByUserId(int id);
    }
}