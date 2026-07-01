namespace backend.DTOs;

public class SearchTourDto
{
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
    public bool IsFavorite { get; set; }
}

public class SearchResultDto
{
    public int TotalCount { get; set; }
    public List<SearchTourDto>? Tours { get; set; } = new();
}

public class SearchLogDto
{
    public Guid LogId { get; set; }
    public Guid TourId { get; set; }
    public string? TourName { get; set; }
    public DateTime DateTime { get; set; }
    public string? Comment { get; set; }
    public string? Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTime { get; set; }
    public int Rating { get; set; }
}

public class SearchLogResultDto
{
    public int TotalCount { get; set; }
    public List<SearchLogDto>? Logs { get; set; } = new();
}