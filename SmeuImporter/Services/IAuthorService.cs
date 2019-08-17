using SmeuImporter.Domain;

namespace SmeuImporter.Services
{
    public interface IAuthorService
    {
        ulong ResolveAuthorId(ChatEntry chatEntry);
    }
}