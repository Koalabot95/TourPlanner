using backend.DTOs;

namespace backend.Interfaces;

public interface IFavoriteService
{
    Task<(bool Success, string? ErrorMessage, int StatusCode)> AddFavoriteAsync(Guid tourId, string userId);
    Task<(bool Success, string? ErrorMessage, int StatusCode)> RemoveFavoriteAsync(Guid tourId, string userId);
    Task<(List<FavoriteTourDto>? Tours, string? ErrorMessage, int StatusCode)> GetFavoritesToursAsync(string userId);
    Task<(bool IsFavorite, int StatusCode)> IsFavoriteTourAsync(Guid tourId, string userId);
}