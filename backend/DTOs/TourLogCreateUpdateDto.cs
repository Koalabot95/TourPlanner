using System;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class TourLogCreateUpdateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Der Name muss zwischen 1 und 100 Zeichen lang sein.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Das Datum und die Uhrzeit sind erforderlich.")]
        public DateTime DateTime { get; set; }

        public string? Comment { get; set; }

        public string? Difficulty { get; set; } // Gültigkeit wird im Service geprüft

        public double TotalDistance { get; set; }

        public double TotalTime { get; set; } 

        [Range(1, 5, ErrorMessage = "Das Rating must zwischen 1 und 5 liegen.")]
        public int Rating { get; set; }
    }
}