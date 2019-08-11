using System.ComponentModel.DataAnnotations;

namespace SmeuBase
{
    public class Suspension
    {
        [Key]
        public int Id { get; set; }

        public ulong User { get; set; }
    }
}
