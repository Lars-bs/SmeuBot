using System;
using System.Threading.Tasks;

namespace SmeuImporter.Services.Implementation
{
    public interface IWhatsAppLogService
    {
        Task<string> ReadNextMessage();
    }

    internal class WhatsAppLogService : IWhatsAppLogService
    {
        public Task<string> ReadNextMessage()
        {
            throw new NotImplementedException();
        }
    }
}