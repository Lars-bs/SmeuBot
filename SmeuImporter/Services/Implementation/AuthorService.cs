using System;
using System.Linq;
using SmeuImporter.Domain;

namespace SmeuImporter.Services.Implementation
{
    public class AuthorService : IAuthorService
    {
        private readonly IFileOperationService fileOperationService;
        private readonly IUserInteractionService userInteractionService;

        public AuthorService(IFileOperationService fileOperationService, IUserInteractionService userInteractionService)
        {
            this.fileOperationService =
                fileOperationService ?? throw new ArgumentNullException(nameof(fileOperationService));
            this.userInteractionService = userInteractionService ??
                                          throw new ArgumentNullException(nameof(userInteractionService));
        }

        public ulong ResolveAuthorId(ChatEntry chatEntry)
        {
            var userMap = fileOperationService.GetUserMap();
            var userId = userMap.SingleOrDefault(map => map.Names.Contains(chatEntry.Author))?.Id ?? 0;
            if (userId != 0) return userId;
            
            userId = userInteractionService.AskForAuthor(chatEntry.Author, userMap);
            userMap.Single(map => map.Id == userId).Names.Add(chatEntry.Author);
            fileOperationService.SetUserMap(userMap);
            return userId;
        }
    }
}