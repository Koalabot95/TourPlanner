using backend.DTOs;

namespace backend.Interfaces;

public interface ISearchService
{
    Task<SearchResultDto> SearchToursAsync(
        string userId,
        string? searchTerm = null,
        string? transportMode = null,
        string? startLocation = null,
        string? endLocation = null,
        int? minPopularity = null,
        int? maxPopularity = null,
        double? minChildFriendliness = null,
        double? maxChildFriendliness = null);
}