using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace backend.Models
{

    public enum TransportMode
    {
        Walking,
        Cycling,
        Driving
    }
    public class Tour
    {
        public Guid TourId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TransportMode TransportType { get; set; } = TransportMode.Walking;
        public double Distance { get; set; }
        public double EstimatedTime { get; set; }
        public string? RouteInformation { get; set; }
        public string? MapSnapshotPath { get; set; }
        public int Popularity { get; set; }
        public double ChildFriendliness { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Eine Tour hat viele TourLogs
        public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();

        // Eine Tour kann viele Bilder in der Galerie haben
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}