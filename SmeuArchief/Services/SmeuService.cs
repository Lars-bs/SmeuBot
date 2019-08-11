using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SmeuArchief.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class SmeuService
    {
        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;
        private readonly Settings settings;
        private readonly LogService logger;

        private readonly Emoji acceptEmoji = new Emoji("\u2705");
        private readonly Emoji denyEmoji = new Emoji("\u274C");

        public SmeuService(IServiceProvider services, DiscordSocketClient client, Settings settings, LogService logger)
        {
            this.services = services;
            this.client = client;
            this.settings = settings;
            this.logger = logger;

            client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            // is this a smeu?
            if (!(arg is SocketUserMessage msg)) { return; }
            if (msg.Author.IsBot) { return; }
            if (msg.Channel.Id != settings.SmeuChannelId) { return; }

            Add(msg).ContinueWith(
                async (t) => await logger.LogAsync(new LogMessage(LogSeverity.Error, "SmeuService", $"Attempted to add smeu to collection, but failed: {t.Exception?.InnerException.Message}\n{t.Exception?.InnerException.StackTrace}")),
                TaskContinuationOptions.OnlyOnFaulted);
        }

        public async Task Add(SocketUserMessage msg)
        {
            // is the user suspended?
            if (IsUserSuspended(msg.Author))
            {
                await msg.DeleteAsync();
                return;
            }

            // has the smeu been submitted before?
            string smeu = msg.Content.ToLowerInvariant();
            Submission submission;
            using (SmeuContext database = services.GetRequiredService<SmeuContext>())
            {
                submission = (from s in database.Submissions
                              where s.Smeu == smeu
                              orderby s.Date
                              select s).FirstOrDefault();
            }

            if (submission != null)
            {
                await msg.AddReactionAsync(denyEmoji);
                await msg.Channel.SendMessageAsync($"{smeu} is al genoemd door {client.GetUser(submission.Author).Mention} op {submission.Date}");
                await msg.Channel.SendMessageAsync($"{msg.Author.Mention} is nu **af**!");

                using (SmeuContext database = services.GetRequiredService<SmeuContext>())
                {
                    database.Suspensions.Add(new Suspension { User = msg.Author.Id });
                    await database.SaveChangesAsync();
                }
            }
            else
            {
                await msg.AddReactionAsync(acceptEmoji);

                using (SmeuContext database = services.GetRequiredService<SmeuContext>())
                {
                    database.Submissions.Add(new Submission { Author = msg.Author.Id, Smeu = smeu, Date = DateTime.UtcNow });
                    await database.SaveChangesAsync();
                }
            }
        }

        private bool IsUserSuspended(SocketUser user)
        {
            // make sure that the sender of the command is not suspended
            bool suspended;
            using (SmeuContext database = services.GetRequiredService<SmeuContext>())
            {
                suspended = (from s in database.Suspensions
                             where s.User == user.Id
                             select s).Any();
            }
            return suspended;
        }
    }
}
