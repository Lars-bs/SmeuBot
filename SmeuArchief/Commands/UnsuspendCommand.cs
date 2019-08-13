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

        public UnsuspendCommand(SmeuService smeuservice)
        {
            this.smeuservice = smeuservice;
        }

        [Command("tik"), Summary("Tik iemand aan zodat ie weer mee mag doen.")]
        public async Task Unsuspend(SocketUser user)
        {
            if(user == Context.User)
            {
                // users are not allowed to unsuspend themselves
                await ReplyAsync("Het is niet toegestaan om jezelf af te tikken, stiekemerd!");
                return;
            }

            if(await smeuservice.UnsuspendAsync(user)) { await ReplyAsync($"{user.Mention} is niet meer af!"); }
            else { await ReplyAsync("Deze gebruiker kan niet afgetikt worden omdat deze niet af is!"); }
        }
    }
}
