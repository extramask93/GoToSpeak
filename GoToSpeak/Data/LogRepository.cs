using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoToSpeak.Helpers;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Identity;

namespace GoToSpeak.Data
{
    public class LogRepository : ILogRepository
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;

        public LogRepository(DataContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async void Add<T>(T entity) where T : class
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async void ClearLogs()
        {
            var logs = context.Logs.ToList();
            foreach(var log in logs) {
            context.Logs.Remove(log);
            }
            await context.SaveChangesAsync();
        }

        public async Task<PagedList<Log>> GetLogs(LogParams logParams)
        {
            var logs  = context.Logs.AsQueryable();
            if(logParams.Level != 0) {
                logs  = logs.Where((log) => log.Level == logParams.Level);
            }
            if(logParams.LastXDays != 0) {
                var dataFrom = DateTime.Now.AddDays(-logParams.LastXDays);
                logs  = logs.Where((log) => log.Timestamp >= dataFrom);
            }
            if(!string.IsNullOrEmpty(logParams.Name)) {
                var user = await userManager.FindByNameAsync(logParams.Name);
                if(user != null) {
                    logs= logs.Where(l => l.UserId == user.Id);
                }
            }
            var list = await PagedList<Log>.CreateAsync(logs,logParams.PageNumber, logParams.PageSize);
            return list;
        }
    }
}