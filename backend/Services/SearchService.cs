using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class SearchService : ISearchService
{
    private readonly TourPlannerContext _context;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(SearchService));

    public SearchService(TourPlannerContext context)
    {
        _context = context;
    }

    public async Task<SearchResultDto> SearchToursAsync(
        string userId,
        string? searchTerm = null,
        string? transportMode = null,
        string? startLocation = null,
        string? endLocation = null,
        int? minPopularity = null,
        int? maxPopularity = null,
        double? minChildFriendliness = null,
        double? maxChildFriendliness = null)
    {
        _logger.Info($"Search Tours for user {userId}: term='{searchTerm}', transport={transportMode}, pop={minPopularity}-{maxPopularity}, cf={minChildFriendliness}-{maxChildFriendliness}");

        var userIdGuid = Guid.Parse(userId);
        var query = _context.Tours.Where(t => t.UserId == userIdGuid);

        // Search: Name + Description (partial match)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term))
            );
            _logger.Info($"Applied search term filter: '{searchTerm}'");
        }

        // Filter: TransportType
        if (!string.IsNullOrWhiteSpace(transportMode))
        {
            query = query.Where(t => t.TransportType.ToString() == transportMode);
            _logger.Info($"Applied transport mode filter: {transportMode}");
        }

        // Filter: StartLocation (exact match)
        if (!string.IsNullOrWhiteSpace(startLocation))
        {
            query = query.Where(t => t.StartLocation == startLocation);
            _logger.Info($"Applied start location filter: {startLocation}");
        }

        // Filter: EndLocation (exact match)
        if (!string.IsNullOrWhiteSpace(endLocation))
        {
            query = query.Where(t => t.EndLocation == endLocation);
            _logger.Info($"Applied end location filter: {endLocation}");
        }

        // Filter: Popularity (min & max)
        if (minPopularity.HasValue)
        {
            query = query.Where(t => t.Popularity >= minPopularity.Value);
            _logger.Info($"Applied min popularity filter: {minPopularity}");
        }

        if (maxPopularity.HasValue)
        {
            query = query.Where(t => t.Popularity <= maxPopularity.Value);
            _logger.Info($"Applied max popularity filter: {maxPopularity}");
        }

        // Filter: ChildFriendliness (min & max)
        if (minChildFriendliness.HasValue)
        {
            query = query.Where(t => t.ChildFriendliness >= minChildFriendliness.Value);
            _logger.Info($"Applied min child-friendliness filter: {minChildFriendliness}");
        }

        if (maxChildFriendliness.HasValue)
        {
            query = query.Where(t => t.ChildFriendliness <= maxChildFriendliness.Value);
            _logger.Info($"Applied max child-friendliness filter: {maxChildFriendliness}");
        }

        // Get all tours
        var tours = await query
            .OrderBy(t => t.Name)
            .ToListAsync();

        _logger.Info($"Search returned {tours.Count} tours");

        var result = new SearchResultDto
        {
            TotalCount = tours.Count,
            Tours = tours.Select(t => new SearchTourDto
            {
                TourId = t.TourId,
                Name = t.Name,
                Description = t.Description ?? "",
                StartLocation = t.StartLocation,
                EndLocation = t.EndLocation,
                Distance = (decimal?)(t.Distance ?? 0) ?? 0,
                EstimatedTime = (int?)(t.EstimatedTime ?? 0) ?? 0,
                TransportMode = t.TransportType.ToString(),
                Popularity = t.Popularity,
                ChildFriendliness = (decimal)t.ChildFriendliness
            }).ToList()
        };

        return result;
    }
}