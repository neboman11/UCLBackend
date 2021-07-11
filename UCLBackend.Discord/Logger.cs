using System;
using Microsoft.Extensions.Logging;

namespace UCLBackend.Discord
{
    public class DiscordLogger : ILogger
    {
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Error)
            {
                Console.WriteLine(formatter(state, exception));
            }
        }
    }
}
