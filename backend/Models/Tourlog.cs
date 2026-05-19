using System;

namespace backend.Models
{
    public class TourLog
    {
        public Guid LogId { get; set; }
        public Guid TourId { get; set; }
        public Tour? Tour { get; set; }
        public string? Name { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comment { get; set; }
        public string? Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public double TotalTime { get; set; } 
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}