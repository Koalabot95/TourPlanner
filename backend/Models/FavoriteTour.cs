namespace backend.Models;

public class FavoriteTour
{
    public Guid FavoriteTourId { get; set; }
    public Guid UserId { get; set; }
    public Guid TourId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Tour? Tour { get; set; }
}