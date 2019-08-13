using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SmeuArchief.Utilities;
using SmeuBase;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmeuArchief.Commands
{
    public partial class SmeuModule : ModuleBase<SocketCommandContext>
    {
        [Group("krijg"), Alias("bekom", "get"), Summary("Commando groep om informatie te krijgen."), Name("Info module")]
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
                using (var typing = Context.Channel.EnterTypingState())
                {
                    using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
                    {
                        // get all suspensions
                        var dbresult = from s in database.Suspensions
                                       select s;

                        // if nobody is suspended, notify the user about that
                        if (dbresult.Count() == 0) { await ReplyAsync("Er is op dit moment helemaal niemand af!"); }
                        else
                        {
                            // present the suspensions in an embed
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
            }

            [Command("smeu"), Summary("Krijg informatie over de gegeven smeu en smeu die er op lijken")]
            public async Task GetSmeu([Remainder, Name("Smeu")]string input)
            {
                input = input.ToLower();

                await GetSmeuReply(input).ConfigureAwait(false);
            }

            private async Task GetSmeuReply(string input)
            {
                using (var typing = Context.Channel.EnterTypingState())
                {
                    Embed embed = GatherSmeuData(input);

                    await ReplyAsync(embed: embed);
                }
            }

            private Embed GatherSmeuData(string input)
            {
                // create embed for given word
                EmbedBuilder eb = new EmbedBuilder()
                    .WithTitle($"__{input}__")
                    .WithColor(Color.LightOrange);

                // find existing submission in database
                Submission submission;
                using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
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
                    eb.AddField("Auteur", user.Username);
                    eb.AddField("Datum", $"{submission.Date:d-MMMM-yyyy H:mm} UTC");
                    eb.AddField("\u200B", "\u200B");
                }

                // add similar smeu to the embed
                using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
                {
                    // find the similar smeu and sort them by similarity
                    var top = from s in database.Submissions
                              let d = Levenshtein.GetLevenshteinDistance(s.Smeu, input)
                              where s.Smeu != input && d <= 4
                              orderby d, s.Smeu
                              select new { Submission = s, Similarity = d };

                    int i = 0;
                    StringBuilder sbsmeu = new StringBuilder();
                    StringBuilder sbsimilarity = new StringBuilder();
                    if (top.Any())
                    {
                        foreach (var r in top)
                        {
                            // add each similar smeu and its similarity value to the embed, up to 5 smeu
                            sbsmeu.AppendLine(r.Submission.Smeu);
                            sbsimilarity.AppendLine($"({Math.Round(Levenshtein.GetSimilarity(r.Submission.Smeu, input, r.Similarity) * 100)}%)");

                            i++;
                            if (i > 4) { break; }
                        }

                        // apply stringbuilders to embed
                        eb.AddField("Vergelijkbaar met:", sbsmeu.ToString(), true);
                        eb.AddField("\u200B", sbsimilarity.ToString(), true);
                    }
                    else
                    {
                        // show it if there are no similar smeu
                        eb.AddField("Vergelijkbaar met:", "*Geen vergelijkbare smeu*");
                    }
                }

                // return the result
                return eb.Build();
            }
        }
    }
}
