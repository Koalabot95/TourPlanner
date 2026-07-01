using backend.DTOs;

namespace backend.Interfaces;

public interface ITourLogSearchService
{
    Task<SearchLogResultDto> SearchLogsAsync(
        string userId,
        string? searchTerm = null,
        string? difficulty = null,
        int? minRating = null,
        int? maxRating = null,
        double? minDistance = null,
        double? maxDistance = null,
        int? minTime = null,
        int? maxTime = null);
}