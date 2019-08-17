﻿using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SmeuBase;
using System;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class RestoreService
    {
        private readonly DiscordSocketClient client;
        private readonly SmeuBaseFactory smeuBaseFactory;
        private readonly LogService logger;

        public RestoreService(DiscordSocketClient client, SmeuBaseFactory smeuBaseFactory, LogService logger)
        {
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
            foreach (var guild in client.Guilds)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", $"Restoring {guild.Name}"));
            }
        }

        public async Task RestoreAsync()
        {
            await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", "Start database migration"));
            try
            {
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Critical, "RestoreService", "Attempted to migrate the database, but failed.", e));
                Environment.Exit(-1);
            }
            await logger.LogAsync(new LogMessage(LogSeverity.Info, "RestoreService", "Database migrated"));
        }
    }
}
