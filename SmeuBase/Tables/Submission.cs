using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmeuBase
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }

        public ulong Author { get; set; }

        [Required]
        public string Smeu { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ulong MessageId { get; set; }

        public ICollection<Duplicate> Duplicates { get; set; }
    }
}
