using System;
using System.ComponentModel.DataAnnotations;

namespace SmeuBase
{
    public class Suspension
    {
        [Key]
        public int Id { get; set; }

        public ulong User { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Reason { get; set; }

        public override string ToString()
        {
            return $"{Date:d-MMMM-yyyy H:mm} UTC → \"{Reason}\"";
        }
    }
}
