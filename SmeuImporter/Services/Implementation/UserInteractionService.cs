﻿using System;
using System.Collections.Generic;
using SmeuImporter.Domain;
using Submission = SmeuBase.Submission;

namespace SmeuImporter.Services.Implementation
{
    public class UserInteractionService:IUserInteractionService
    {
        private const string ChoiceText = " Enter 'y' for yes and 'n' for no.";
        
        public bool IsSmeu(ChatEntry chatEntry)
        {
            Console.Clear();
            Console.WriteLine("Parsed the following Chat Entry.");
            Console.WriteLine($"Message: {chatEntry.Message}");
            return AskUserYesOrNo($"Please decided if this is a Smeu.");

        }

        private static bool AskUserYesOrNo(string message)
        {
            do
            {
                Console.WriteLine(message + ChoiceText);
                var input = Console.ReadLine();
                
                if (input == "y") return true;
                if (input == "n") return false;
                
            } while (true);
        }
        
        public Submission ConfirmChatEntryData(ChatEntry chatEntry)
        {
            Console.Clear();
            Console.WriteLine($"Is this Smeu in a valid format? {ChoiceText}");
            var choice = AskUserYesOrNo($"Is Smeu: {chatEntry.Message} in a valid format?");
            if(choice) return CreateSubmission(chatEntry.DateTime, chatEntry.Message);
            
            bool validEntry;
            string smeuInCorrectedFormat;
            do
            {
                Console.WriteLine("Please Enter the correct Smeu in a Correct Format");
                smeuInCorrectedFormat = Console.ReadLine();
                validEntry = AskUserYesOrNo($"You entered: {smeuInCorrectedFormat} is this correct?");

            } while (validEntry);
            
            Console.WriteLine("Added your change to the submission.");
            return CreateSubmission(chatEntry.DateTime, smeuInCorrectedFormat);
        }

        private static Submission CreateSubmission(DateTime dateTime, string smeu)
        {
            return new Submission
            {
                Date = dateTime,
                Smeu = smeu
            };
        }

        public bool IsUniqueSmeu(IReadOnlyCollection<SmeuBase.Submission> fuzzySearchResults, Submission submission)
        {
            Console.WriteLine($"Searched the database for Smeu's like: {submission.Smeu}, found the following results");
            foreach (var fuzzySearchResult in fuzzySearchResults)
            {
                Console.WriteLine($"{fuzzySearchResult.Smeu}, ");
            }

            return AskUserYesOrNo("Is this Smeu Unique?");
        }
    }
}