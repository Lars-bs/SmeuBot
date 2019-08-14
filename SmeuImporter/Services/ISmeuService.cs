using System.Collections.Generic;
using System.Threading.Tasks;
using SmeuBase;

namespace SmeuImporter.Services
{
    public interface ISmeuService
    {
        Task<IReadOnlyCollection<SmeuBase.Submission>> SearchForSmeu(SmeuBase.Submission submission);
        Task AddSmeu(Submission submission);
    }
}