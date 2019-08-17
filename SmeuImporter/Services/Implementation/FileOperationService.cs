using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SmeuImporter.Domain;

namespace SmeuImporter.Services.Implementation
{
    public class FileOperationService:IFileOperationService
    {
        private readonly string filePath =  System.AppDomain.CurrentDomain.BaseDirectory + "/userids.json";
        
        public List<User> GetUserMap()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, List<dynamic>>>(
                    File.ReadAllText(filePath))
                ["users"].Select(dynamic => new User
                {
                    Id = ulong.Parse(dynamic.id.ToString()),
                    Names = dynamic.names.ToObject<List<string>>()
                }).ToList();
        }

        public void SetUserMap(List<User> userMap)
        {
            var userMapAsJson = JsonConvert.SerializeObject(userMap);
            if(File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllText(filePath,userMapAsJson);
        }
    }
}