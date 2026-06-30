using backend.Models;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class TourDto
{
    public Guid? TourId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public string StartLocation { get; set; } = null!;

    [Required]
    public string EndLocation { get; set; } = null!;

    [Required(ErrorMessage = "Das Startdatum ist erforderlich.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Das Enddatum ist erforderlich.")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Der Transporttyp ist erforderlich.")]
    [EnumDataType(typeof(TransportMode))]
    public TransportMode TransportType { get; set; }

    // Readonly 
    public double? Distance { get; set; }
    public double? EstimatedTime { get; set; }
    public int Popularity { get; set; }
    public double ChildFriendliness { get; set; }
    public string? MapSnapshotPath { get; set; }
    public string? RouteInformation { get; set; }
}