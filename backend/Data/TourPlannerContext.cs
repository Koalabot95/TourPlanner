using Microsoft.EntityFrameworkCore;
using backend.Models;
namespace backend.Data
{
    public class TourPlannerContext : DbContext
    {
        public TourPlannerContext(DbContextOptions<TourPlannerContext> options) : base(options) { }

        // Definieren der Tabllen
        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<FavoriteTour> FavoriteTours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PostgreSQL Extension für UUIDs sicherstellen
            modelBuilder.HasPostgresExtension("pgcrypto");


            //User Mapping
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users"); // Mapping auf kleingeschriebenen Tabellennamen
                entity.HasKey(u => u.UserId);
                // Primary Key & ID Generierung
                entity.Property(u => u.UserId)
                    .HasColumnName("user_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                // Pflichtfelder & Constraints
                entity.Property(u => u.Username)
                    .HasColumnName("username")
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasIndex(u => u.Username).IsUnique();

                entity.Property(u => u.Email)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("password_hash")
                    .IsRequired();

                // Optionale Felder
                entity.Property(u => u.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(50);

                entity.Property(u => u.Bio)
                    .HasColumnName("bio");

                // Zeitstempel
                entity.Property(u => u.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            //Tour Mapping
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("tours");
                entity.HasKey(t => t.TourId);
                // Primary & Foreign Keys
                entity.Property(t => t.TourId).HasColumnName("tour_id").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(t => t.UserId).HasColumnName("user_id");

                // Strings und Text
                entity.Property(t => t.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(t => t.Description).HasColumnName("description");
                entity.Property(t => t.StartLocation).HasColumnName("start_location").HasMaxLength(255);
                entity.Property(t => t.EndLocation).HasColumnName("end_location").HasMaxLength(255);
                entity.Property(t => t.RouteInformation).HasColumnName("route_information");
                entity.Property(t => t.MapSnapshotPath).HasColumnName("map_snapshot_path");

                // Dates & Times
                entity.Property(t => t.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(t => t.EndDate).HasColumnName("end_date").IsRequired();
                entity.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(t => t.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Enums & Numbers
                entity.Property(t => t.TransportType).HasColumnName("transport_type").HasConversion<string>();
                entity.Property(t => t.Distance).HasColumnName("distance");
                entity.Property(t => t.EstimatedTime).HasColumnName("estimated_time");
                entity.Property(t => t.Popularity).HasColumnName("popularity").HasDefaultValue(0);
                entity.Property(t => t.ChildFriendliness).HasColumnName("child_friendliness").HasDefaultValue(0.0);

                // Beziehung User -> Tours
                entity.HasOne<User>() //Eine Tour gehört zu einem User
                      .WithMany() //Ein User kann viele Touren haben
                      .HasForeignKey(t => t.UserId) //FK: UserId
                      .OnDelete(DeleteBehavior.Cascade); //Wenn User gelöscht, werden alle seine Touren gelöscht

                // Beziehung Tour -> TourLogs 
                entity.HasMany(t => t.TourLogs) // Eine Tour hat viele TourLogs
                      .WithOne(l => l.Tour)     // Ein TourLog gehört zu einer Tour
                      .HasForeignKey(l => l.TourId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Beziehung Tour -> Images
                entity.HasMany(t => t.Images)   // Eine Tour kann viele Bilder haben
                      .WithOne(i => i.Tour)     // Ein Bild verweist zurück auf die Tour
                      .HasForeignKey(i => i.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            //TourLog Mapping
            modelBuilder.Entity<TourLog>(entity =>
            {

                entity.ToTable("tour_logs");
                entity.HasKey(l => l.LogId);

                // Primary & Foreign Key
                entity.Property(l => l.LogId)
                    .HasColumnName("log_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(l => l.TourId)
                    .HasColumnName("tour_id");

                // Weitere Felder 
                entity.Property(l => l.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(l => l.DateTime)
                    .HasColumnName("date_time")
                    .IsRequired();

                entity.Property(l => l.Comment)
                    .HasColumnName("comment");

                entity.Property(l => l.Difficulty)
                    .HasColumnName("difficulty")
                    .HasMaxLength(50);

                entity.Property(l => l.TotalDistance)
                    .HasColumnName("total_distance");

                entity.Property(l => l.TotalTime)
                    .HasColumnName("total_time");

                entity.Property(l => l.Rating)
                    .HasColumnName("rating");

                entity.Property(l => l.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Beziehung Tourlog -> Images
                entity.HasMany(l => l.Images)   // Ein Log kann viele Bilder haben
                      .WithOne(i => i.TourLog)  // Ein Bild verweist zurück auf das Log
                      .HasForeignKey(i => i.LogId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //Image Mapping
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("images");
                entity.HasKey(i => i.ImageId);
                // ID & Foreign Key
                entity.Property(i => i.ImageId)
                    .HasColumnName("image_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(i => i.TourId)
                      .HasColumnName("tour_id").IsRequired(false); // Optional

                entity.Property(i => i.LogId)
                      .HasColumnName("log_id").IsRequired(false);   // Optional

                // Pfad & Beschreibung
                entity.Property(i => i.FilePath)
                    .HasColumnName("file_path")
                    .IsRequired();

                entity.Property(i => i.Caption)
                    .HasColumnName("caption");

                // Zeitstempel
                entity.Property(i => i.UploadedAt)
                    .HasColumnName("uploaded_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            });

            // FavoriteTour Mapping
            modelBuilder.Entity<FavoriteTour>(entity =>
            {
                entity.ToTable("favorite_tours");
                entity.HasKey(f => f.FavoriteTourId);

                entity.Property(f => f.FavoriteTourId)
                    .HasColumnName("favorite_tour_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(f => f.UserId)
                    .HasColumnName("user_id");

                entity.Property(f => f.TourId)
                    .HasColumnName("tour_id");

                entity.Property(f => f.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Beziehung zu User
                entity.HasOne(f => f.User)
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Beziehung zu Tour
                entity.HasOne(f => f.Tour)
                    .WithMany()
                    .HasForeignKey(f => f.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique Index: User kann eine Tour nur einmal favorisieren
                entity.HasIndex(f => new { f.UserId, f.TourId })
                    .IsUnique();
            });

        }
    }
}