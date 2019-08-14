using System.Threading.Tasks;
using SmeuImporter.Domain;

namespace SmeuImporter.Services
{
    public interface IChatEntryReviewService
    {
        ChatEntry? Parse(string chatLineToParse);
    }
}