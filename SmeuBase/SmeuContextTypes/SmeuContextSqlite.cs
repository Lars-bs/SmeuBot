using Microsoft.EntityFrameworkCore;

namespace SmeuBase
{
    internal class SmeuContextSqlite : SmeuContext
    {
#if DB_BUILD
        public SmeuContextSqlite() : base() { }
#endif

        public SmeuContextSqlite(IContextSettingsProvider contextSettings) : base(contextSettings) { }

        public SmeuContextSqlite(DbContextOptions<SmeuContextSqlite> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
#endif
#if DB_BUILD
                optionsBuilder.UseSqlite("Data Source=Smeubase.db");
#else
                optionsBuilder.UseSqlite(contextSettings.ConnectionString);
#endif
            }
        }
    }
}
