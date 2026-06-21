namespace backend.DTOs;

public class SearchTourDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string StartLocation { get; set; } = null!;
    public string EndLocation { get; set; } = null!;
    public decimal Distance { get; set; }
    public int EstimatedTime { get; set; }
    public string TransportMode { get; set; } = null!;
    public int Popularity { get; set; }
    public decimal ChildFriendliness { get; set; }
}

public class SearchResultDto
{
    public int TotalCount { get; set; }
    public List<SearchTourDto> Tours { get; set; } = new();
}