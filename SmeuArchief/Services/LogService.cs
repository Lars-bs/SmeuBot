using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class LogService
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        public LogService(DiscordSocketClient client, CommandService commands)
        {
            this.client = client;
            this.commands = commands;

            this.client.Log += LogAsync;
            this.commands.Log += LogAsync;
        }

        public async Task LogAsync(LogMessage arg)
        {
            await Console.Out.WriteAsync($"[{arg.Severity.ToString().PadLeft(8)}] {DateTime.UtcNow} [{arg.Source.PadRight(15)}] {arg.Message}");
            await Console.Out.WriteLineAsync(arg.Exception != null ? $" {arg.Exception.Message}\n{arg.Exception.StackTrace}" : "");
        }
    }
}
