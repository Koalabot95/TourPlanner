namespace backend.DTOs;

public class FavoriteTourDto
{
    public Guid FavoriteTourId { get; set; }
    public Guid TourId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? StartLocation { get; set; }
    public string? EndLocation { get; set; }
    public decimal Distance { get; set; }
    public int EstimatedTime { get; set; }
    public string? TransportMode { get; set; }
    public int Popularity { get; set; }
    public decimal ChildFriendliness { get; set; }
    public DateTime AddedAt { get; set; }
}