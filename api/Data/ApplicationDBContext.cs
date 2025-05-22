using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<AudioFile> AudioFiles { get; set; }
        public DbSet<Customization> Customizations { get; set; }
        public DbSet<CloudStorage> CloudStorages { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Setlist> Setlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SetlistSong>()
                .HasKey(ss => new { ss.SetlistId, ss.SongId });

            modelBuilder.Entity<SetlistSong>()
                .HasOne(ss => ss.Setlist)
                .WithMany(s => s.SetlistSongs)
                .HasForeignKey(ss => ss.SetlistId);

            modelBuilder.Entity<SetlistSong>()
                .HasOne(ss => ss.Song)
                .WithMany(s => s.SetlistSongs)
                .HasForeignKey(ss => ss.SongId);
        }

    }
}
