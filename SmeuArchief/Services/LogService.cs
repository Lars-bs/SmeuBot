using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class LogService
    {
        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        public LogService(IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            this.services = services;
            this.client = client;
            this.commands = commands;

            client.Log += LogAsync;
            commands.Log += LogAsync;
        }

        public async Task LogAsync(LogMessage arg)
        {
            await Console.Out.WriteLineAsync($"[{arg.Severity.ToString().PadLeft(8)}] {DateTime.UtcNow} [{arg.Source.PadRight(15)}] {arg.Message}");
        }
    }
}
