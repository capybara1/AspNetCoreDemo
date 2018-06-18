using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Utils
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper)
        {
            builder.AddProvider(new XunitLoggerProvider(testOutputHelper));
            return builder;
        }

        public static ILoggingBuilder SetupDemoLogging(this ILoggingBuilder builder, ITestOutputHelper testOutputHelper)
        {
            builder.AddXunit(testOutputHelper);
            builder.SetMinimumLevel(LogLevel.Information);
            return builder;
        }
    }
}
