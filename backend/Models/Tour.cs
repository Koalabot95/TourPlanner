using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Tour
    {
        [Key]
        public Guid TourId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TransportType { get; set; }
        public double Distance { get; set; }
        public double EstimatedTime { get; set; }
        public string? RouteInformation { get; set; }
        public string? MapSnapshotPath { get; set; }
        public int Popularity { get; set; }
        public double ChildFriendliness { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}