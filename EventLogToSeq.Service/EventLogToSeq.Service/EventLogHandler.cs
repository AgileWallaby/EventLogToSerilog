using System;
using System.Diagnostics;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace AgileWallaby.EventLogToSeq.Service
{
    public class EventLogHandler: IDisposable
    {
        private readonly EventLog _eventLog;
        private readonly ILogger _logger;
        private string[] _includedSources;

        public EventLogHandler(string logName)
        {
            _eventLog = new EventLog(logName) { EnableRaisingEvents = true };
            _eventLog.EntryWritten += EventLogOnEntryWritten;

            _logger = Log.ForContext("Log Name", logName);
        }

        public string[] IncludedSources
        {
            get => _includedSources;
            set { _includedSources = value.OrderBy(x=>x).ToArray(); }
        }

        private void EventLogOnEntryWritten(object sender, EntryWrittenEventArgs eventArgs)
        {
            var eventLogEntry = eventArgs.Entry;

            if (IncludedSources != null && IncludedSources.Length > 0)
            {
                if (Array.BinarySearch(IncludedSources, eventLogEntry.Source) == -1)
                {
                    return;
                }
            }

            var eventLogEntryType = eventLogEntry.EntryType;

            LogEventLevel level;
            switch (eventLogEntryType)
            {
                case EventLogEntryType.Error:
                    level = LogEventLevel.Error;
                    break;
                case EventLogEntryType.Warning:
                    level = LogEventLevel.Warning;
                    break;
                case EventLogEntryType.Information:
                    level = LogEventLevel.Information;
                    break;
                case EventLogEntryType.SuccessAudit:
                    level = LogEventLevel.Information;
                    break;
                case EventLogEntryType.FailureAudit:
                    level = LogEventLevel.Information;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _logger
                .ForContext(new EventLogEntryEnricher(eventLogEntry))
                .Write(level, "{Message}", eventLogEntry.Message);
        }

        public void Dispose()
        {
            _eventLog?.Dispose();
        }
    }
}