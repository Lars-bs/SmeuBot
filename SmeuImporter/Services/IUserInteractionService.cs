using System.Collections.Generic;
using SmeuImporter.Domain;
using Submission = SmeuBase.Submission;

namespace SmeuImporter.Services
{
    public interface IUserInteractionService
    {
        bool IsSmeu(ChatEntry chatEntry);
        Submission ConfirmChatEntryData(ChatEntry chatEntry);
        bool IsUniqueSmeu(IReadOnlyCollection<SmeuBase.Submission> fuzzySearchResults, Submission submission);
        ulong AskForAuthor(string parsedAuthor, List<User> userMap);
    }
}