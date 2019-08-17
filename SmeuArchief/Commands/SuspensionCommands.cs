using Discord.Commands;
using Discord.WebSocket;
using SmeuBase;
using System.Threading.Tasks;
using System.Linq;

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

                if (await smeuService.UnsuspendAsync(user.Id, Context.User.Id)) { await ReplyAsync($"{user.Mention} is niet meer af!"); }
                else { await ReplyAsync("Deze gebruiker kan niet afgetikt worden omdat deze niet af is!"); }
            }
        }

        [Command("af"), Summary("Gebruik deze als je denkt dat iemand af is!")]
        public async Task Suspend([Name("Gebruiker")]SocketUser user, [Name("Smeu"), Remainder]string smeu)
        {
            using(var typing = Context.Channel.EnterTypingState())
            {
                Duplicate duplicate;
                using(SmeuContext database = smeuBaseFactory.GetSmeuBase())
                {
                    duplicate = (from d in database.Duplicates
                                 where d.Author == user.Id && d.Suspension == null && d.Original.Smeu == smeu
                                 select d).FirstOrDefault();
                }

                if(duplicate == null)
                {
                    await ReplyAsync("Nee, dat klopt niet.");
                    return;
                }

                if(await smeuService.SuspendAsync(user.Id, Context.User.Id, $"{smeu} is al eerder genoemd.", duplicate))
                {
                    await ReplyAsync($"{user.Mention} is nu **af**!");
                }
                else
                {
                    await ReplyAsync($"{user.Username} is al af!");
                }
            }
        }
    }
}
