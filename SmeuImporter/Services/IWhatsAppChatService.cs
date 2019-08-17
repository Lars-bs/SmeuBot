using System.Threading.Tasks;

namespace SmeuImporter.Services
{
    public interface IWhatsAppChatService
    {
        Task EvaluateChat(string whatsAppChatFilePath);
    }
}