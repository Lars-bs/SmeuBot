using System;
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
    }
}
