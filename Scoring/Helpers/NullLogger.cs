using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

// copied from https://github.com/aspnet/Logging/blob/master/src/Microsoft.Extensions.Logging.Abstractions/NullLoggerOfT.cs
namespace Citolab.QTI.ScoringEngine.Helpers
{
    /// <summary>
    /// Minimalistic logger that does nothing.
    /// </summary>
    public class NullLogger<T> : ILogger<T>
    {
        public static readonly NullLogger<T> Instance = new NullLogger<T>();

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This method ignores the parameters and does nothing.
        /// </remarks>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();

            public void Dispose()
            {
                // intentionally does nothing
            }
        }
    }
}
