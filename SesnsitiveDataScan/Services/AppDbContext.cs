using Microsoft.EntityFrameworkCore;

namespace SesnsitiveDataScan.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.FlaggedItem> FlaggedItems { get; set; }
        public DbSet<Models.ScannedFile> ScannedFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=SensitiveDataScan.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.FlaggedItem>()
                .HasOne(f => f.ScannedFile)
                .WithMany(s => s.FlaggedItems)
                .HasForeignKey(f => f.ScannedFileId);

            modelBuilder.Entity<Models.ScannedFile>()
                .HasMany(s => s.FlaggedItems)
                .WithOne(f => f.ScannedFile)
                .HasForeignKey(f => f.ScannedFileId);
        }
    }
}
