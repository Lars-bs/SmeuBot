using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public partial class SmeuModule : ModuleBase<SocketCommandContext>
    {
        [Command("tik"), Summary("Tik iemand aan zodat ie weer mee mag doen.")]
        public async Task Unsuspend([Name("Gebruiker")]SocketUser user)
        {
            using (var typing = Context.Channel.EnterTypingState())
            {
                if (user == Context.User)
                {
                    // users are not allowed to unsuspend themselves
                    await ReplyAsync("Het is niet toegestaan om jezelf af te tikken, stiekemerd!");
                    return;
                }

                if (await smeuService.UnsuspendAsync(user)) { await ReplyAsync($"{user.Mention} is niet meer af!"); }
                else { await ReplyAsync("Deze gebruiker kan niet afgetikt worden omdat deze niet af is!"); }
            }
        }
    }
}
