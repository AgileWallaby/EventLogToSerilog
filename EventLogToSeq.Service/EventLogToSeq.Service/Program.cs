using Topshelf;

namespace AgileWallaby.EventLogToSerilog.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configurator =>
            {
                configurator.Service<PumpService>(svc =>
                {
                    svc.ConstructUsing(() => new PumpService());
                    svc.WhenStarted(pump => pump.Start());
                    svc.WhenStopped(pump => pump.Stop());
                });

                configurator.SetServiceName("EventLogToSeq");
                configurator.SetDescription("Pumps messages from Windows Event Log into the Serilog logging framework.");
            });
        }
    }
}

