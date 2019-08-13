using Discord;
using Discord.WebSocket;
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
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            // is this a smeu?
            if (!(arg is SocketUserMessage msg)) { return; }
            if (msg.Author.IsBot) { return; }
            if (msg.Channel.Id != settings.SmeuChannelId) { return; }

            // is user allowed to submit a smeu?
            if (GetUserSuspension(arg.Author) != null)
            {
                await msg.DeleteAsync();
                return;
            }

            // create submission from message
            Submission submission = new Submission { Author = msg.Author.Id, Date = msg.CreatedAt.UtcDateTime, MessageId = msg.Id, Smeu = msg.Content.ToLower() };
            Submission result;

            // try to add it to the database
            if ((result = await Add(submission)) != submission)
            {
                await msg.AddReactionAsync(denyEmoji);
                await msg.Channel.SendMessageAsync($"{submission.Smeu} is al genoemd door {client.GetUser(result.Author).Mention} op {result.Date}");

                if (!await SuspendAsync(msg.Author, $"{submission.Smeu} is al genoemd.")) { await msg.Channel.SendMessageAsync("Deze gebruiker is al af!"); }
                else { await msg.Channel.SendMessageAsync($"{msg.Author.Mention} is **af**!"); }
            }
            else
            {
                await msg.AddReactionAsync(acceptEmoji);
            }
        }

        public async Task<bool> SuspendAsync(SocketUser user, string reason)
        {
            if (GetUserSuspension(user) != null)
            {
                // if there is a suspension, return feedback to user
                return false;
            }
            else
            {
                // if there is no suspension, add one to the database
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Suspensions.Add(new Suspension { User = user.Id, Date = DateTime.UtcNow, Reason = reason });
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }

        public async Task<bool> UnsuspendAsync(SocketUser user)
        {
            Suspension suspension;
            if ((suspension = GetUserSuspension(user)) == null)
            {
                // if there is no suspension, return feedback to the user
                return false;
            }
            else
            {
                // if there is a suspension, remove it from the database
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Suspensions.Remove(suspension);
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }

        public async Task<Submission> Add(Submission submission)
        {
            // has the smeu been submitted before?
            Submission dbresult;
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                dbresult = (from s in database.Submissions
                            where s.Smeu == submission.Smeu
                            orderby s.Date
                            select s).FirstOrDefault();
            }

            if (dbresult != null)
            {
                // return the result if there is already such a smeu
                return dbresult;
            }

            // add the submission to the database if none existed yet
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                database.Submissions.Add(submission);
                await database.SaveChangesAsync();
            }
            return submission;
        }

        public async Task Remove(Submission submission)
        {

            try
            {
                // try remove the submission from the discord
                IMessage message = await (client.GetChannel(settings.SmeuChannelId) as IMessageChannel).GetMessageAsync(submission.MessageId);
                if (message != null)
                {
                    await message.DeleteAsync();
                }
                else
                {
                    await logger.LogAsync(new LogMessage(LogSeverity.Warning, "SmeuService", $"Attempted to delete message from discord with id '{submission.MessageId}', but failed: Could not find the message."));
                }
            }
            catch (Exception e)
            {
                await logger.LogAsync(new LogMessage(LogSeverity.Warning, "SmeuService", $"Attempted to delete message from discord with id '{submission.MessageId}', but failed.", e));
            }

            // remove from database
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                database.Submissions.RemoveRange(submission);
                await database.SaveChangesAsync();
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
