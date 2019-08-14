using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SmeuImporter.Domain;
using Sprache;

namespace SmeuImporter.Services.Implementation
{
    public class ChatEntryReviewService : IChatEntryReviewService
    {
        private readonly ILogger<ChatEntryReviewService> logger;

        public ChatEntryReviewService(ILogger<ChatEntryReviewService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public  ChatEntry? Parse(string chatLineToParse)
        {
            logger.LogInformation($"Starting Parsing of Line: {chatLineToParse}");
            var regex = new Regex("[^A-Z,a-z]{1}[a,f,A,F]{2}[^A-Z,a-z]{1}");
            if (regex.IsMatch(chatLineToParse))
            {
                logger.LogInformation("Chat line contains an af! Skipping this line");
                return null;
            }

            var chatEntryParser =
                from dateTime in ChatEntryGrammar.DateTime
                from author in ChatEntryGrammar.Author
                from description in ChatEntryGrammar.Description
                select new ChatEntry {DateTime = dateTime};
            var result = chatEntryParser.TryParse(chatLineToParse);
            
            if (!result.WasSuccessful)
            {
                logger.LogInformation("Parsing of line Failed skipping this line");
                Console.WriteLine($"Parsing of line Failed. Line: {chatLineToParse}");
                return null;
            }
            
            Console.WriteLine("Parsing of line Successful.");
            logger.LogInformation($"Parsing of line successful result {result.Value}");
            return result.Value;
        }
    }
}