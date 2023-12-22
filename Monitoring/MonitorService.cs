using System.Diagnostics;
using System.Reflection;
using Serilog;
using Serilog.Enrichers.Span;


namespace Monitoring;

public class MonitorService
{
    public static ILogger Log => Serilog.Log.Logger;

    static MonitorService()
    {
        // Serilog
        Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.WithSpan()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:5341")
            .CreateLogger();

    }

}
