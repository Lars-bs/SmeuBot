using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace SmeuImporter.Services.Implementation
{
    public class WhatsAppChatService:IWhatsAppChatService
    {
        private readonly IChatEntryReviewService reviewService;
        private readonly IUserInteractionService userInteractionService;
        private readonly ISmeuService smeuService;
        private readonly IAuthorService authorService;
        private readonly ILogger logger;

        public WhatsAppChatService(IChatEntryReviewService reviewService, 
            IUserInteractionService userInteractionService, ISmeuService smeuService, IAuthorService authorService,
            ILogger logger)
        {
            this.reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            this.userInteractionService = userInteractionService ?? throw new ArgumentNullException(nameof(userInteractionService));
            this.smeuService = smeuService ?? throw new ArgumentNullException(nameof(smeuService));
            this.authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task EvaluateChat(string whatsAppChatFilePath)
        {
            logger.Info($"Starting to evaluate chat directory {whatsAppChatFilePath}");
            var filesInDirectory = Directory.GetFiles(whatsAppChatFilePath);
            var whatsAppChatFile = filesInDirectory.Single(file => file.Contains(".txt"));
            logger.Info($"Found whatsApp chat log file: {whatsAppChatFile}");
            var stream = File.OpenText(whatsAppChatFile);
            
            while(!stream.EndOfStream)
            {
                var chatLineToParse = await stream.ReadLineAsync();
                var chatEntry = reviewService.Parse(chatLineToParse);
                
                if (chatEntry is null)
                {
                    logger.Info($"Could not parse line: {chatLineToParse} skipping to the next line");
                    continue;
                }

                if (!userInteractionService.IsSmeu(chatEntry))
                {
                    logger.Info($"User decided that {chatEntry.Message} is not a Smeu continueing to next line");
                    continue;
                }
                
                var submission = userInteractionService.ConfirmChatEntryData(chatEntry);
                logger.Info($"Submission confirmed as: {submission}");
                
                var fuzzySearchResults = await smeuService.SearchForSmeu(submission);
                if(!userInteractionService.IsUniqueSmeu(fuzzySearchResults, submission)) continue;
               
                submission.Author = authorService.ResolveAuthorId(chatEntry);
                await smeuService.AddSmeu(submission);
            }
        }
    }
}