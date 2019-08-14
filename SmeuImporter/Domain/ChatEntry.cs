using System;

namespace SmeuImporter.Domain
{
    public class ChatEntry
    {
        public DateTime DateTime { get; set; } = DateTime.MaxValue.Date;
        public string Author { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}