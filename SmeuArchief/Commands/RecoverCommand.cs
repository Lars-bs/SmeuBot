using Discord.Commands;
using SmeuArchief.Services;
using SmeuBase;
using System.Linq;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public partial class SmeuModule : ModuleBase<SocketCommandContext>
    {
        private readonly SmeuService smeuService;
        private readonly SmeuBaseFactory smeuBaseFactory;

        public SmeuModule(SmeuService smeuService, SmeuBaseFactory smeuBaseFactory)
        {
            this.smeuService = smeuService;
            this.smeuBaseFactory = smeuBaseFactory;
        }

        [Command("herstel"), Summary("Als je per ongeluk een woord verkeerd hebt geschreven, dan kun je hem hiermee weer weghalen.")]
        public async Task Recover([Remainder, Name("Smeu")]string input)
        {
            using (var typing = Context.Channel.EnterTypingState())
            {
                input = input.ToLower();

                // find the referenced smeu
                Submission submission;
                using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
                {
                    submission = (from s in database.Submissions
                                  where s.Smeu == input && s.Author == Context.User.Id
                                  select s).FirstOrDefault();
                }

                // make sure that the smeu exists
                if (submission == null)
                {
                    await ReplyAsync("Jij hebt die smeu niet ingediend, dus ik kan m ook niet verwijderen.");
                    return;
                }

                // remove it from the database
                await smeuService.Remove(submission);
                await ReplyAsync($"'{submission.Smeu}' is niet langer meer een smeu.");
            }
        }
    }
}
