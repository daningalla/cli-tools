using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Vertical.SpectreLogger;

namespace Vertical.HeaderUtility;

public static class AppLogger
{
    public static ILogger Default { get; set; } = NullLogger.Instance;

    public static ILogger Initialize()
    {
        Default = LoggerFactory
            .Create(logging => logging
                .SetMinimumLevel(LogLevel.Trace)
                .AddSpectreConsole(console => console
                    .SetMinimumLevel(LogLevel.Trace)
                    .ConfigureProfile(LogLevel.Debug, p => p.OutputTemplate = "[grey50]{Message}{NewLine}{Exception}[/]")
                    .ConfigureProfile(LogLevel.Information, p => p.OutputTemplate = "[grey85]{Message}{NewLine}{Exception}[/]")
                )
            )
            .CreateLogger("Main");

        return Default;
    }
}