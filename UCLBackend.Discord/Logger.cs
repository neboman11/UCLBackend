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
            Console.WriteLine(formatter(state, exception));
            Console.WriteLine($"Exception: {exception.Message}");

            if (exception.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {exception.InnerException.Message}");
            }

            Console.WriteLine(exception.StackTrace);
        }
    }
}
