using Discord;
using Discord.Commands;
using SmeuArchief.Services;
using System;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public class SmeuCommands : ModuleBase<SocketCommandContext>
    {
        private readonly SmeuService smeuService;
        private readonly LogService logger;

        public SmeuCommands(SmeuService smeuService, LogService logger)
        {
            this.smeuService = smeuService;
            this.logger = logger;
        }

        [Command("zend"), Alias("z")]
        [Summary("voegt de gegeven smeu toe aan de collectie. PAS OP! Als ie al genoemd is, dan ben je af!")]
        public async Task Add(string smeu)
        {
            try
            {
                await smeuService.Add(Context, smeu);
            }
            catch(Exception e)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Error, "zend", $"Attempted to perform command, but failed: {e.Message}\n{e.StackTrace}"));
            }
        }
    }
}
