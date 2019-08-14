using System;
using System.Collections.Generic;
using System.Text;

namespace SmeuBase
{
    public class Duplicate
    {
        public int Id { get; set; }

        public ulong Author { get; set; }

        public DateTime Date { get; set; }

        public ulong MessageId { get; set; }

        public Submission Original { get; set; }

        public int OriginalId { get; set; }

        public Suspension Suspension { get; set; }

        public int? SuspensionId { get; set; }
    }
}
