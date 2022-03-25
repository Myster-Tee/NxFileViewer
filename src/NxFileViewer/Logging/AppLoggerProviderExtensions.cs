using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Logging;

public static class AppLoggerProviderExtensions
{
    public static ILoggingBuilder AddAppLoggerProvider(this ILoggingBuilder builder)
    {
        builder.Services
            .AddSingleton<AppLoggerProvider>()
            .AddSingleton<ILoggerProvider>(x => x.GetRequiredService<AppLoggerProvider>())
            .AddSingleton<IAppLoggerProvider>(x => x.GetRequiredService<AppLoggerProvider>())
            .AddSingleton<ILogSource>(x => x.GetRequiredService<AppLoggerProvider>());
        return builder;
    }

}