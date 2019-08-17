using System.Collections.Generic;

namespace SmeuImporter.Domain
{
    public class User
    {
        public ulong Id { get; set; }
        public List<string> Names { get; set; } = new List<string>();
    }
}