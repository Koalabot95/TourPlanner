using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("tour_logs")]
    public class TourLog
    {
        [Key]
        [Column("log_id")]
        public Guid LogId { get; set; }

        [Required]
        [Column("tour_id")]
        public Guid TourId { get; set; }

        // Navigation Property für den Berechtigungs-Check im Service
        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }

        [Column("name")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [Column("date_time")]
        public DateTime DateTime { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("difficulty")]
        [StringLength(50)]
        public string? Difficulty { get; set; } // "Easy", "Moderate", "Challenging", "Hard"

        [Column("total_distance")]
        public double TotalDistance { get; set; }

        [Column("total_time")]
        public double TotalTime { get; set; } 

        [Column("rating")]
        public int Rating { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}