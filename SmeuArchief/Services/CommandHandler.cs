﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class CommandHandler
    {
        private readonly IServiceProvider services;
        private readonly CommandService commands;
        private readonly DiscordSocketClient client;
        private readonly LogService logger;
        private readonly Settings settings;

        public CommandHandler(IServiceProvider services,
                                CommandService commands,
                                DiscordSocketClient client,
                                LogService logger,
                                Settings settings)
        {
            this.services = services;
            this.commands = commands;
            this.client = client;
            this.logger = logger;
            this.settings = settings;

            client.MessageReceived += ReceiveMessageAsync;
        }

        private async Task ReceiveMessageAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg)) { return; }
            if (msg.Author.IsBot) { return; }

            SocketCommandContext context = new SocketCommandContext(client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(settings.CommandPrefix, ref argPos))
            {
                IResult result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess) { await logger.LogAsync(new LogMessage(LogSeverity.Warning, "CommandHandler", $"Attempted to execute command, but failed: {result.ErrorReason}")); }
            }
        }
    }
}
