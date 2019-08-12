using Microsoft.EntityFrameworkCore;

namespace SmeuBase
{
    public abstract class SmeuContext : DbContext
    {
        protected readonly IContextSettingsProvider contextSettings;

        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }

#if DB_BUILD
        public SmeuContext() : base() { }
#endif

        public SmeuContext(IContextSettingsProvider contextSettings) : base()
        {
            this.contextSettings = contextSettings;
        }
        public SmeuContext(DbContextOptions options) : base(options) { }
    }
}
