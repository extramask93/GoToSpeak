using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoToSpeak.Models;

namespace GoToSpeak.Data
{
    public class LogRepository : ILogRepository
    {
        private readonly DataContext context;

        public LogRepository(DataContext context)
        {
            this.context = context;
        }

        public async void Add<T>(T entity) where T : class
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
        }


        public Task<IEnumerable<Log>> GetAllLogs()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Log>> GetLogsByMinDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Log>> GetLogsBySeverity(int severity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Log>> GetLogsByUserId(int id)
        {
            throw new NotImplementedException();
        }
    }
}