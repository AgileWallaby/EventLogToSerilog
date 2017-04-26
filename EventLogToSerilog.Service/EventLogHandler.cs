using System;
using System.Diagnostics;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace AgileWallaby.EventLogToSerilog.Service
{
    public class EventLogHandler: IDisposable
    {
        private readonly PumpConfiguration.Log _logConfig;
        private readonly EventLog _eventLog;
        private readonly ILogger _logger;

        public EventLogHandler(PumpConfiguration.Log logConfig)
        {
            _logConfig = logConfig;
            _eventLog = string.IsNullOrWhiteSpace(logConfig.MachineName) ? new EventLog(logConfig.LogName) : new EventLog(logConfig.LogName, logConfig.MachineName);
            _eventLog.EnableRaisingEvents = true;

            _eventLog.EntryWritten += EventLogOnEntryWritten;

            _logger = Log.ForContext("Log Name", logConfig.LogName);
        }

        private void EventLogOnEntryWritten(object sender, EntryWrittenEventArgs eventArgs)
        {
            var entry = eventArgs.Entry;

            if (IsExcludedByFilter(entry)) { return; }

            var logLevel = TranslateWindowsEventLogLevelToSerilogLogLevel(entry);

            _logger
                .ForContext(new EventLogEntryEnricher(entry))
                .Write(logLevel, "{Message}", entry.Message);
        }

        private bool IsExcludedByFilter(EventLogEntry eventLogEntry)
        {
            var sourceConfig = _logConfig.Sources.FirstOrDefault(x => x.Name == eventLogEntry.Source);
            if (sourceConfig == null)
            {
                return true;
            }

            if (sourceConfig.EventIds == null || sourceConfig.EventIds.Length <= 0) { return false; }
            return Array.IndexOf(sourceConfig.EventIds, eventLogEntry.InstanceId) == -1;
        }

        private static LogEventLevel TranslateWindowsEventLogLevelToSerilogLogLevel(EventLogEntry eventLogEntry)
        {
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
            return level;
        }

        public void Dispose()
        {
            _eventLog?.Dispose();
        }
    }
}