using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SmeuBase;
using SmeuArchief.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text;

namespace SmeuArchief.Commands
{
    [Group("krijg"), Alias("bekom", "get"), Summary("Commando groep om informatie te krijgen.")]
    public class GetCommands : ModuleBase<SocketCommandContext>
    {
        private readonly SmeuBaseFactory smeuBaseFactory;
        private readonly DiscordSocketClient client;

        public GetCommands(SmeuBaseFactory smeuBaseFactory, DiscordSocketClient client)
        {
            this.smeuBaseFactory = smeuBaseFactory;
            this.client = client;
        }

        [Command("schorsingen"), Alias("suspensions"), Summary("Krijg informatie over wie er nu af is.")]
        public async Task GetSuspensions()
        {

            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                var dbresult = from s in database.Suspensions
                               select s;

                if (dbresult.Count() == 0) { await ReplyAsync("Er is op dit moment helemaal niemand af!"); }
                else
                {
                    EmbedBuilder eb = new EmbedBuilder()
                        .WithTitle("Deze mensen zijn **af**:")
                        .WithColor(Color.DarkRed);
                    foreach (Suspension suspension in dbresult)
                    {
                        eb.AddField(client.GetUser(suspension.User).Username, suspension);
                    }
                    await ReplyAsync(embed: eb.Build());
                }
            }
        }

        [Command("smeu"), Summary("Krijg informatie over de gegeven smeu en smeu die er op lijken")]
        public async Task GetSmeu([Remainder]string input)
        {
            input = input.ToLower();

            // create embed for given word
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle($"__{input}__")
                .WithColor(Color.LightOrange);

            // find existing submission in database
            Submission submission;
            using(SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                submission = (from s in database.Submissions
                              where s.Smeu == input
                              select s).FirstOrDefault();
            }

            // add potential existing submission to the embed
            if (submission != null)
            {
                SocketUser user = client.GetUser(submission.Author);

                eb = eb.WithThumbnailUrl(user.GetAvatarUrl());
                eb.AddField("Auteur", user.Username, true);
                eb.AddField("Datum", $"{submission.Date:d-MMMM-yyyy H:mm} UTC", true);
            }

            // add similar smeu to the embed
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                var top = from s in database.Submissions
                          let d = Levenshtein.GetSimilarity(s.Smeu, input)
                          where s.Smeu != input
                          orderby d descending, s.Smeu
                          select new { Submission = s, Similarity = d };

                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach(var r in top)
                {
                    sb.AppendLine($"{r.Submission.Smeu} ({Math.Round(r.Similarity * 100)}% hetzelfde)");

                    i++;
                    if(i > 4) { break; }
                }

                eb.AddField("Vergelijkbaar met:", sb.ToString());

                await ReplyAsync(embed: eb.Build());
            }
        }
    }
}
