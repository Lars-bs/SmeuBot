using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SmeuBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class SmeuService
    {
        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;
        private readonly SmeuBaseFactory smeuBaseFactory;
        private readonly Settings settings;
        private readonly LogService logger;

        private readonly Emoji acceptEmoji = new Emoji("\u2705");
        private readonly Emoji denyEmoji = new Emoji("\u274C");

        public SmeuService(IServiceProvider services, DiscordSocketClient client, SmeuBaseFactory smeuBaseFactory, Settings settings, LogService logger)
        {
            this.services = services;
            this.client = client;
            this.smeuBaseFactory = smeuBaseFactory;
            this.settings = settings;
            this.logger = logger;

            client.MessageReceived += Client_MessageReceived;
            client.MessageDeleted += Client_MessageDeleted;
        }

        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            if (channel.Id != settings.SmeuChannelId) { return; }
            IMessage msg = await message.GetOrDownloadAsync();

            if (msg == null)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Warning, "SmeuService", "Could not retrieve deleted message!"));
                return;
            }

            Submission submission;
            using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
            {
                submission = (from s in context.Submissions
                              where s.Author == (msg.Author.Id) && s.Smeu == msg.Content.ToLower() && s.Date == msg.CreatedAt.UtcDateTime
                              select s).FirstOrDefault();
            }

            if (submission != null)
            {
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Submissions.Remove(submission);
                    await context.SaveChangesAsync();
                }
                await logger.LogAsync(new LogMessage(LogSeverity.Info, "SmeuService", $"Smeu '{msg.Content}' was deleted."));
            }

        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            // is this a smeu?
            if (!(arg is SocketUserMessage msg)) { return; }
            if (msg.Author.IsBot) { return; }
            if (msg.Channel.Id != settings.SmeuChannelId) { return; }

            await Add(msg);
        }

        public async Task SuspendAsync(SocketUser user, ISocketMessageChannel responseChannel)
        {
            if (GetUserSuspension(user) != null)
            {
                // if there is a suspension, return feedback to user
                await responseChannel.SendMessageAsync("Deze gebruiker is al af!");
            }
            else
            {
                // if there is no suspension, add one to the database
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Suspensions.Add(new Suspension { User = user.Id });
                    await context.SaveChangesAsync();
                }
                await responseChannel.SendMessageAsync($"{user.Mention} is **af**!");
            }
        }

        public async Task UnsuspendAsync(SocketUser user, ISocketMessageChannel responseChannel)
        {
            Suspension suspension;
            if ((suspension = GetUserSuspension(user)) == null)
            {
                // if there is no suspension, return feedback to the user
                await responseChannel.SendMessageAsync("Deze gebruiker kan niet afgetikt worden omdat deze niet af is!");
            }
            else
            {
                // if there is a suspension, remove it from the database
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Suspensions.Remove(suspension);
                    await context.SaveChangesAsync();
                }
                await responseChannel.SendMessageAsync($"{user.Mention} is niet meer af!");
            }
        }

        public async Task Add(SocketUserMessage msg)
        {
            // is the user suspended?
            if (GetUserSuspension(msg.Author) != null)
            {
                await msg.DeleteAsync();
                return;
            }

            // has the smeu been submitted before?
            string smeu = msg.Content.ToLowerInvariant();
            Submission submission;
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
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
                await SuspendAsync(msg.Author, msg.Channel);
            }
            else
            {
                await msg.AddReactionAsync(acceptEmoji);

                using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
                {
                    database.Submissions.Add(new Submission { Author = msg.Author.Id, Smeu = smeu, Date = msg.CreatedAt.UtcDateTime });
                    await database.SaveChangesAsync();
                }
            }
        }

        private Suspension GetUserSuspension(SocketUser user)
        {
            // check if there is an entry in the suspensions table for given user
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                return (from s in database.Suspensions
                        where s.User == user.Id
                        select s).FirstOrDefault();
            }
        }
    }
}
