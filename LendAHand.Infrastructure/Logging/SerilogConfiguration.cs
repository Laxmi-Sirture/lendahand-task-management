using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Logging
{
    public static class SerilogConfiguration
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    "logs/log-.txt",
                    rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
