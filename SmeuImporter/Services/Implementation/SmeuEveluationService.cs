using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmeuBase;

namespace SmeuImporter.Services.Implementation
{
    public class SmeuEvaluationService:ISmeuEvaluationService
    {
        private readonly IChatEntryReviewService reviewService;
        private readonly IUserInteractionService userInteractionService;
        private readonly ISmeuService smeuService;

        public SmeuEvaluationService(IChatEntryReviewService reviewService, 
            IUserInteractionService userInteractionService, ISmeuService smeuService, IAuthorService authorService)
        {
            this.reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            this.userInteractionService = userInteractionService ?? throw new ArgumentNullException(nameof(userInteractionService));
            this.smeuService = smeuService ?? throw new ArgumentNullException(nameof(smeuService));
        }
        public async Task EvaluateChat(string whatsAppChatFilePath)
        {
            var filesInDirectory = Directory.GetFiles(whatsAppChatFilePath);
            var whatsAppChatFile = filesInDirectory.Single(file => file.Contains(".txt"));
            var stream = File.OpenText(whatsAppChatFile);
            
            while(!stream.EndOfStream)
            {
                var chatLineToParse = await stream.ReadLineAsync();
                var chatEntry = reviewService.Parse(chatLineToParse);
                if(chatEntry is null) continue;
                
                if(!userInteractionService.IsSmeu(chatEntry)) continue;
                var submission = userInteractionService.ConfirmChatEntryData(chatEntry);
                
                var fuzzySearchResults = await smeuService.SearchForSmeu(submission);
                if(!userInteractionService.IsUniqueSmeu(fuzzySearchResults, submission)) continue;
                
                
                await smeuService.AddSmeu(submission);
            }
        }
    }

    public interface IAuthorService
    {
    }
}