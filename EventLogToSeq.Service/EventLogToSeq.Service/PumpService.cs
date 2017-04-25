using Serilog;

namespace AgileWallaby.EventLogToSeq.Service
{
    public class PumpService
    {
        private EventLogHandler _handler;

        public void Start()
        {
            // Using Serilog.Config.AppSettings?
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            //TODO: Create an array of handlers from some configuration file.
            _handler = new EventLogHandler("Application") {IncludedSources = new[] {"Outlook"}};
        }

        public void Stop()
        {
            _handler.Dispose();
        }
    }
}