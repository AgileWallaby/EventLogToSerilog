namespace AgileWallaby.EventLogToSerilog.Service
{
    public class PumpConfiguration
    {
        public Log[] Logs { get; set; }

        public class Log
        {
            public string LogName { get; set; }
            public string MachineName { get; set; }

            public Source[] Sources { get; set; }
        }

        public class Source
        {
            public string Name { get; set; }
            public long[] EventIds { get; set; }
        }
        
    }
}