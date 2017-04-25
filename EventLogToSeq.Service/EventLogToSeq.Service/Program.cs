using Topshelf;

namespace AgileWallaby.EventLogToSeq.Service
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
            });
        }
    }
}

