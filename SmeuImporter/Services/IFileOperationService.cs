using System.Collections.Generic;
using SmeuImporter.Domain;

namespace SmeuImporter.Services
{
    public interface IFileOperationService
    {
        List<User> GetUserMap();
        void SetUserMap(List<User> userMap);
    }
}