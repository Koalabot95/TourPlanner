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
        int skip = 0,
        int take = 10)
    {
        _logger.Info($"Search Tours for user {userId}: term='{searchTerm}', transport={transportMode}");

        var userIdGuid = Guid.Parse(userId);
        var query = _context.Tours.Where(t => t.UserId == userIdGuid);

        // Search: Name + Description (LIKE - partial match)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term))
            );
            _logger.Info($"Applied search term filter: '{searchTerm}'");
        }

        // Filter: TransportType (not TransportMode!)
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

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var tours = await query
            .OrderBy(t => t.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        _logger.Info($"Search returned {tours.Count} tours (total: {totalCount})");

        var result = new SearchResultDto
        {
            TotalCount = totalCount,
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