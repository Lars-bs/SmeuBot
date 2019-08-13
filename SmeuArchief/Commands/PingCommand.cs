using Discord.Commands;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("ping"), Summary("Returns a message")]
        public async Task Ping()
        {
            await ReplyAsync($"Hallo {Context.User.Mention}!");
        }
    }
}
