using System;
using GoToSpeak.Data;
using GoToSpeak.Models;
using Microsoft.Extensions.Logging;

namespace GoToSpeak.Helpers
{
    public interface IDbLogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void Log(int level, string message);
    }
    public class Logger : IDbLogger
    {
    private readonly ILogRepository repo;
    public Logger(ILogRepository repo) 
    {
        this.repo = repo;
    }
    public void LogError(string message) {
        Log(3, message);
    }
    public void LogInfo(string message) {
        Log(1, message);
    }
    public void LogWarning(string message) {
        Log(2,message);
    }
    public void Log(int level, string message) {
        Log eventLog = new Log 
        { 
            Message = message, 
            Level = level,
            Timestamp = DateTime.UtcNow 
        }; 
        Console.Write(message);
        repo.Add(eventLog);
    } 
}
}