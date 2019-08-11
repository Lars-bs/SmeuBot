using Microsoft.EntityFrameworkCore;

namespace SmeuArchief.Database
{
    public class SmeuContext : DbContext
    {
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }

        public SmeuContext() : base() { }
        public SmeuContext(DbContextOptions<SmeuContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlite($"Data Source=Database.db"); }
        }
    }
}
