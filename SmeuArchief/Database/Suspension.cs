using System.ComponentModel.DataAnnotations;

namespace SmeuArchief.Database
{
    public class Suspension
    {
        [Key]
        public int Id { get; set; }

        public ulong User { get; set; }
    }
}
