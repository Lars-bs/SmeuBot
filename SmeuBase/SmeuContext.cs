using Microsoft.EntityFrameworkCore;
using System;

namespace SmeuBase
{
    public class SmeuContext : DbContext
    {
        private readonly IContextSettingsProvider contextSettings;

        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }

        public SmeuContext(IContextSettingsProvider contextSettings) : base()
        {
            this.contextSettings = contextSettings;
        }
        public SmeuContext(DbContextOptions<SmeuContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                switch (contextSettings.DbType)
                {
                    case DbType.Sqlite:
                        optionsBuilder.UseSqlite(contextSettings.ConnectionString);
                        break;
                    case DbType.MySql:
                        optionsBuilder.UseMySQL(contextSettings.ConnectionString);
                        break;
                    default:
                        throw new ArgumentException("Given database type is unknown!");
                }
            }
        }
    }
}
