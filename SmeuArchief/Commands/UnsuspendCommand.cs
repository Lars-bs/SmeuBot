using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SmeuArchief.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public class UnsuspendCommand : ModuleBase<SocketCommandContext>
    {
        private readonly SmeuService smeuservice;
        private readonly LogService logger;

        public UnsuspendCommand(SmeuService smeuservice, LogService logger)
        {
            this.smeuservice = smeuservice;
            this.logger = logger;
        }

        [Command("tik"), Summary("Tik iemand aan zodat ie weer mee mag doen.")]
        public Task Unsuspend(SocketUser user)
        {
            if(user == Context.User)
            {
                return Context.Channel.SendMessageAsync("Het is niet toegestaan om jezelf af te tikken, stiekemerd!");
            }

            return smeuservice.UnsuspendAsync(user, Context.Channel)
                .ContinueWith(
                async (t) => await logger.LogAsync(new LogMessage(LogSeverity.Error, "SmeuService", $"Attempted to add smeu to collection, but failed: {t.Exception?.InnerException.Message}\n{t.Exception?.InnerException.StackTrace}")),
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
