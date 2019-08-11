using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SmeuArchief.Database;
using System;
using System.Threading.Tasks;
using System.Linq;
using Discord;

namespace SmeuArchief.Services
{
    public class SmeuService
    {
        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;

        private readonly Emoji acceptEmoji = new Emoji("\u2705");
        private readonly Emoji denyEmoji = new Emoji("\u274C");

        public SmeuService(IServiceProvider services, DiscordSocketClient client)
        {
            this.services = services;
            this.client = client;
        }

        public async Task Add(SocketCommandContext context, string smeu)
        {
            // is the user suspended?
            if (IsUserSuspended(context.User))
            {
                await context.Message.DeleteAsync();
                return;
            }

            // has the smeu been submitted before?
            smeu = smeu.ToLowerInvariant();
            Submission submission;
            using(SmeuContext database = services.GetRequiredService<SmeuContext>())
            {
                submission = (from s in database.Submissions
                              where s.Smeu == smeu
                              orderby s.Date
                              select s).FirstOrDefault();
            }

            if(submission != null)
            {
                await context.Message.AddReactionAsync(denyEmoji);
                await context.Channel.SendMessageAsync($"{smeu} is al genoemd door {client.GetUser(submission.Author).Mention} op {submission.Date}");
                await context.Channel.SendMessageAsync($"{context.User.Mention} is nu **af**!");

                using (SmeuContext database = services.GetRequiredService<SmeuContext>())
                {
                    database.Suspensions.Add(new Suspension { User = context.User.Id });
                    await database.SaveChangesAsync();
                }
            }
            else
            {
                await context.Message.AddReactionAsync(acceptEmoji);

                using (SmeuContext database = services.GetRequiredService<SmeuContext>())
                {
                    database.Submissions.Add(new Submission { Author = context.User.Id, Smeu = smeu, Date = DateTime.UtcNow });
                    await database.SaveChangesAsync();
                }
            }
        }

        private bool IsUserSuspended(SocketUser user)
        {
            // make sure that the sender of the command is not suspended
            bool suspended;
            using(SmeuContext database = services.GetRequiredService<SmeuContext>())
            {
                suspended = (from s in database.Suspensions
                             where s.User == user.Id
                             select s).Any();
            }
            return suspended;
        }
    }
}
