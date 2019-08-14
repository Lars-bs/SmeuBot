using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace SmeuImporter.Services.Implementation
{
    public static class ServicesFactory
    {
        public static void Configure(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IMain, Main>();
            serviceCollection.AddScoped<IChatEntryReviewService, ChatEntryReviewService>();
            serviceCollection.AddScoped<ISmeuEvaluationService, SmeuEvaluationService>();
            
            serviceCollection 
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    loggingBuilder.AddNLog(configuration);
                });
        }
    }
}