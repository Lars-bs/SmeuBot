﻿using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SmeuBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmeuArchief.Services
{
    public class SmeuService
    {
        private readonly DiscordSocketClient client;
        private readonly SmeuBaseFactory smeuBaseFactory;
        private readonly Settings settings;
        private readonly LogService logger;

        private readonly Emoji acceptEmoji = new Emoji("\u2705");
        private readonly Emoji denyEmoji = new Emoji("\u274C");

        public SmeuService(DiscordSocketClient client, SmeuBaseFactory smeuBaseFactory, Settings settings, LogService logger)
        {
            this.client = client;
            this.smeuBaseFactory = smeuBaseFactory;
            this.settings = settings;
            this.logger = logger;

            client.MessageReceived += SaveSmeuAsync;
        }

        private async Task SaveSmeuAsync(SocketMessage arg)
        {
            // is this a smeu?
            if (!(arg is SocketUserMessage msg)) { return; }
            if (msg.Author.IsBot) { return; }
            if (msg.Channel.Id != settings.SmeuChannelId) { return; }

            // is user allowed to submit a smeu?
            if (GetUserSuspension(arg.Author.Id) != null)
            {
                await msg.DeleteAsync();
                return;
            }

            // add it to the database
            await AddAsync(msg.Content.ToLower(), msg.CreatedAt.UtcDateTime, msg.Author.Id, msg.Id);
        }

        public async Task<bool> SuspendAsync(ulong user, ulong suspender, string reason, Duplicate duplicate = null)
        {
            if (GetUserSuspension(user) != null)
            {
                // if user is already suspended, return feedback to user
                return false;
            }

            // if there is no suspension, add one to the database
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                Suspension suspension = new Suspension { User = user, Date = DateTime.UtcNow, Suspender = suspender, Reason = reason };
                database.Suspensions.Add(suspension);
                if (duplicate != null)
                {
                    // if a duplicate was given, update the duplicate to reflect the suspension
                    duplicate.Suspension = suspension;
                    database.Duplicates.Update(duplicate);
                }
                await database.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> UnsuspendAsync(ulong user, ulong revoker)
        {
            Suspension suspension;
            if ((suspension = GetUserSuspension(user)) == null)
            {
                // if there is no suspension, return feedback to the user
                return false;
            }
            else
            {
                // if there is a suspension, add the revoker to it.
                suspension.Revoker = revoker;
                using (SmeuContext context = smeuBaseFactory.GetSmeuBase())
                {
                    context.Suspensions.Update(suspension);
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }

        public async Task<bool> AddAsync(string smeu, DateTime date, ulong author, ulong messageid)
        {
            // has the smeu been submitted before?
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                Submission dbresult = (from s in database.Submissions
                                       where s.Smeu == smeu
                                       orderby s.Date
                                       select s).FirstOrDefault();


                if (dbresult != null)
                {
                    // create a duplicate if an original already exists
                    database.Duplicates.Add(new Duplicate { Author = author, Date = date, MessageId=messageid, Original = dbresult });
                    await database.SaveChangesAsync();
                    return false;
                }
                else
                {
                    // otherwise add it to the database
                    database.Submissions.Add(new Submission { Author = author, Date = date, MessageId = messageid, Smeu = smeu });
                    await database.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<bool> RemoveAsync(string smeu, ulong author)
        {
            using(SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                // first check if this combination is a duplicate
                Duplicate duplicate = (from d in database.Duplicates
                                       where d.Author == author && d.Original.Smeu == smeu
                                       orderby d.Date descending
                                       select d).FirstOrDefault();

                if(duplicate != null)
                {
                    // remove the duplicate
                    IMessage msg = await (client.GetChannel(settings.SmeuChannelId) as IMessageChannel).GetMessageAsync(duplicate.MessageId);
                    await msg.DeleteAsync();

                    database.Duplicates.Remove(duplicate);
                    await database.SaveChangesAsync();
                    return true;
                }

                // check if there is an original
                Submission submission = await (from s in database.Submissions
                                               where s.Author == author && s.Smeu == smeu
                                               select s).Include(x => x.Duplicates).FirstOrDefaultAsync();

                if(submission != null)
                {
                    // remove the original message
                    IMessage msg = await (client.GetChannel(settings.SmeuChannelId) as IMessageChannel).GetMessageAsync(submission.MessageId);
                    await msg.DeleteAsync();

                    // check if a duplicate must take this submission's place
                    if(submission.Duplicates.Count > 0)
                    {
                        duplicate = (from d in submission.Duplicates
                                     orderby d.Date ascending
                                     select d).First();

                        // change details of this submission to the first duplicate
                        submission.Author = duplicate.Author;
                        submission.Date = duplicate.Date;
                        submission.MessageId = duplicate.MessageId;

                        database.Submissions.Update(submission);
                        database.Duplicates.Remove(duplicate);
                    }
                    else
                    {
                        database.Submissions.Remove(submission);
                    }

                    await database.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }

        private Suspension GetUserSuspension(ulong user)
        {
            // check if there is an entry in the suspensions table for given user
            using (SmeuContext database = smeuBaseFactory.GetSmeuBase())
            {
                return (from s in database.Suspensions
                        where s.Revoker == null && s.User == user
                        select s).FirstOrDefault();
            }
        }
    }
}
