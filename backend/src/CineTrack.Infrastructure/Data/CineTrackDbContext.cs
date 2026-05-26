using CineTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineTrack.Infrastructure.Data
{
    public class CineTrackDbContext : DbContext
    {
        public CineTrackDbContext(DbContextOptions<CineTrackDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StreamingProvider> StreamingProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .Property(u => u.FavoriteGenres)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            // Watchlist configuration
            modelBuilder.Entity<Watchlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Watchlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // WatchlistItem configuration
            modelBuilder.Entity<WatchlistItem>()
                .HasOne(wi => wi.Watchlist)
                .WithMany(w => w.Items)
                .HasForeignKey(wi => wi.WatchlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchlistItem>()
                .HasOne(wi => wi.Movie)
                .WithMany(m => m.WatchlistItems)
                .HasForeignKey(wi => wi.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review configuration
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .Property(r => r.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            // ReviewComment configuration
            modelBuilder.Entity<ReviewComment>()
                .HasOne(rc => rc.Review)
                .WithMany(r => r.Comments)
                .HasForeignKey(rc => rc.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReviewComment>()
                .HasOne(rc => rc.User)
                .WithMany()
                .HasForeignKey(rc => rc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Follow configuration
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Badge configuration
            modelBuilder.Entity<Badge>()
                .HasMany(b => b.Users)
                .WithMany(u => u.Badges);

            // Movie configuration
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.TmdbId)
                .IsUnique();

            modelBuilder.Entity<Movie>()
                .Property(m => m.Genres)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<Movie>()
                .Property(m => m.Cast)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            // Notification configuration
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // StreamingProvider configuration
            modelBuilder.Entity<StreamingProvider>()
                .HasOne(sp => sp.Movie)
                .WithMany(m => m.StreamingProviders)
                .HasForeignKey(sp => sp.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}