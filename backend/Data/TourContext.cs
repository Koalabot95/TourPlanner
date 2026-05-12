using Microsoft.EntityFrameworkCore; 
using backend.Models;                
namespace backend.Data
{
    public class TourContext : DbContext
    {
        public TourContext(DbContextOptions<TourContext> options) : base(options) { }

        // Definieren der Tabllen
        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EF Core mitteilen, dass PostgreSQL UUIDs generiert.
            
            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Tour>()
                .Property(t => t.TourId)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<TourLog>()
                .Property(l => l.LogId)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Image>()
                .Property(i => i.ImageId)
                .HasDefaultValueSql("gen_random_uuid()");

            //User -> Tours
            modelBuilder.Entity<Tour>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Tour-Log -> Tours
            modelBuilder.Entity<TourLog>()
                .HasOne<Tour>()
                .WithMany()
                .HasForeignKey(l => l.TourId)
                .OnDelete(DeleteBehavior.Cascade);

                // Image -> TourLog 
            modelBuilder.Entity<Image>()
                .HasOne<TourLog>()
                .WithMany()
                .HasForeignKey(i => i.LogId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}