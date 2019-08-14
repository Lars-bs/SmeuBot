using System.Threading.Tasks;

namespace SmeuImporter.Services
{
    public interface ISmeuEvaluationService
    {
        Task EvaluateChat(string whatsAppChatFilePath);
    }
}