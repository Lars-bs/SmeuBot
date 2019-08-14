using Microsoft.EntityFrameworkCore;

namespace SmeuBase
{
    public abstract class SmeuContext : DbContext
    {
        protected readonly IContextSettingsProvider contextSettings;

        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }
        public DbSet<Duplicate> Duplicates { get; set; }

#if DB_BUILD
        public SmeuContext() : base() { }
#endif

        public SmeuContext(IContextSettingsProvider contextSettings) : base()
        {
            this.contextSettings = contextSettings;
        }
        public SmeuContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Duplicate>()
                .HasOne(d => d.Original)
                .WithMany(s => s.Duplicates)
                .HasForeignKey(d => d.OriginalId)
                .HasConstraintName("ForeignKey_Duplicate_Submission")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Duplicate>()
                .HasOne(d => d.Suspension)
                .WithOne(s => s.Duplicate)
                .HasForeignKey<Duplicate>(d => d.SuspensionId)
                .HasConstraintName("ForeignKey_Duplicate_Suspension")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
