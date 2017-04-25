using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace AgileWallaby.EventLogToSeq.Service
{
    public class EventLogEntryEnricher : ILogEventEnricher
    {
        private readonly EventLogEntry _eventLogEntry;

        public EventLogEntryEnricher(EventLogEntry eventLogEntry)
        {
            _eventLogEntry = eventLogEntry;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.RemovePropertyIfPresent("EventLogEntry");
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Source", _eventLogEntry.Source));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Event ID", _eventLogEntry.InstanceId));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("MachineName", _eventLogEntry.MachineName));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TimeGenerated", _eventLogEntry.TimeGenerated));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TimeWritten", _eventLogEntry.TimeWritten));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Category", _eventLogEntry.Category));
            logEvent.AddOrUpdateProperty(
                propertyFactory.CreateProperty("ReplacementStrings", _eventLogEntry.ReplacementStrings));
        }
    }
}