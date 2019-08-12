using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmeuBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class RestoreService
    {
        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;
        private readonly SmeuBaseFactory smeuBaseFactory;
        private readonly LogService logger;

        public RestoreService(IServiceProvider services, DiscordSocketClient client, SmeuBaseFactory smeuBaseFactory, LogService logger)
        {
            this.services = services;
            this.client = client;
            this.smeuBaseFactory = smeuBaseFactory;
            this.logger = logger;

            client.Ready += SetStateMessageAsync;
            client.Ready += RestorePendingSmeuAsync;
        }

        private async Task SetStateMessageAsync()
        {
            await client.SetGameAsync("all your activities", type: ActivityType.Watching);
        }

        private async Task RestorePendingSmeuAsync()
        {
            foreach(var guild in client.Guilds)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", $"Restoring {guild.Name}"));
            }
        }

        public async Task RestoreAsync()
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", "Start database migration"));
            using(SmeuContext context = smeuBaseFactory.GetSmeuBase())
            {
                context.Database.Migrate();
            }
            await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", "Database migrated"));
        }
    }
}
