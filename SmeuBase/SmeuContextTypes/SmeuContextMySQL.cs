using Microsoft.EntityFrameworkCore;

namespace SmeuBase
{
    public class SmeuContextMySQL : SmeuContext
    {
#if DB_BUILD
        public SmeuContextMySQL() : base() { }
#endif

        public SmeuContextMySQL(IContextSettingsProvider contextSettings) : base(contextSettings) { }

        public SmeuContextMySQL(DbContextOptions<SmeuContextMySQL> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
#endif
#if DB_BUILD
                optionsBuilder.UseMySQL("server=localhost;database=smeubase");
#else
                optionsBuilder.UseMySQL(contextSettings.ConnectionString);
#endif
            }
        }
    }
}
