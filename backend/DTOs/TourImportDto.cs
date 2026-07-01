using backend.Models;

namespace backend.DTOs;

public class TourImportDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? StartLocation { get; set; }
    public string? EndLocation { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TransportMode TransportType { get; set; }
    public double? Distance { get; set; }
    public double? EstimatedTime { get; set; }
    public string? RouteInformation { get; set; }
    public List<TourLogImportDto>? Logs { get; set; }
}

public class TourLogImportDto
{
    public DateTime DateTime { get; set; }
    public string? Comment { get; set; }
    public string? Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public int TotalTime { get; set; }
    public int Rating { get; set; }
}