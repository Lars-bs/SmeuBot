using System;
using System.Collections.Generic;
using SmeuBase;
using SmeuImporter.Domain;

namespace SmeuImporter.Tests
{
    public static class SmeuTestDataFactory
    {
        public static User NewUser(ulong id = 1, List<string> names = default)
        {
            return new User
            {
                Id = id,
                Names = names
            };
        }

        public static Submission NewSubmission(ulong author = 1, DateTime date = default, string smeu = "smeu")
        {
            return new Submission
            {
                Author = author,
                Date = date,
                Smeu = smeu
            };
        }

        public static ChatEntry NewChatEntry(string author = "lars",DateTime dateTime = default, string message = "smeu")
        {
            return new ChatEntry
            {
                Author = author,
                DateTime = dateTime,
                Message = message
            };
        }
    }
}