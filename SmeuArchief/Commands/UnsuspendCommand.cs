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
        public async Task Unsuspend(SocketUser user)
        {
            if(user == Context.User)
            {
                // users are not allowed to unsuspend themselves
                await Context.Channel.SendMessageAsync("Het is niet toegestaan om jezelf af te tikken, stiekemerd!");
                return;
            }

            await smeuservice.UnsuspendAsync(user, Context.Channel);
        }
    }
}
