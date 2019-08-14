using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DuoVia.FuzzyStrings;
using Microsoft.EntityFrameworkCore;
using SmeuBase;

namespace SmeuImporter.Services.Implementation
{
    public class SmeuService : ISmeuService
    {
        private readonly SmeuContext smeuContext;

        public SmeuService(SmeuContext smeuContext)
        {
            this.smeuContext = smeuContext ?? throw new ArgumentNullException(nameof(smeuContext));
        }

        public async Task<IReadOnlyCollection<Submission>> SearchForSmeu(Submission submission)
        {
            return new ReadOnlyCollection<Submission>(
                await smeuContext
                    .Submissions
                    .Where(sub => sub.Smeu.LevenshteinDistance(submission.Smeu, false) < 4)
                    .ToArrayAsync());
        }
        
        public async Task AddSmeu(Submission submission)
        {
            var smeuDao = new Submission
            {
                Date = submission.Date,
                Smeu = submission.Smeu
            };
            smeuContext.Submissions.Add(smeuDao);
            await smeuContext.SaveChangesAsync();
        }
    }
}