using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AgileWallaby.EventLogToSerilog.Service
{
    public class PumpService
    {
        private List<EventLogHandler> _handlers;

        public void Start()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("service-settings.json");

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            ConfigurePump(configuration);
        }

        private void ConfigurePump(IConfiguration configuration)
        {
            var config = new PumpConfiguration();
            configuration.GetSection("EventLog").Bind(config);

            _handlers = new List<EventLogHandler>();
            foreach (var log in config.Logs)
            {
                var handler = new EventLogHandler(log);
                _handlers.Add(handler);
            }
        }

        public void Stop()
        {
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
        }
    }
}