using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TourLogSearchService : ITourLogSearchService
{
    private readonly TourPlannerContext _context;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(TourLogSearchService));

    public TourLogSearchService(TourPlannerContext context)
    {
        _context = context;
    }

    public async Task<SearchLogResultDto> SearchLogsAsync(
        string userId,
        string? searchTerm = null,
        string? difficulty = null,
        int? minRating = null,
        int? maxRating = null,
        double? minDistance = null,
        double? maxDistance = null,
        int? minTime = null,
        int? maxTime = null)
    {
        _logger.Info($"Search Logs for user {userId}: term='{searchTerm}', difficulty={difficulty}, rating={minRating}-{maxRating}");

        var userIdGuid = Guid.Parse(userId);

        // Nur Logs von Tours des Users!
        var query = _context.TourLogs
            .Where(l => l.Tour != null && l.Tour.UserId == userIdGuid);

        // Search: Comment (partial match)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(l =>
                (l.Comment != null && l.Comment.ToLower().Contains(term)) ||
                (l.Tour != null && l.Tour.Name.ToLower().Contains(term))
            );
            _logger.Info($"Applied search term filter: '{searchTerm}'");
        }

        // Filter: Difficulty
        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            query = query.Where(l => l.Difficulty == difficulty);
            _logger.Info($"Applied difficulty filter: {difficulty}");
        }

        // Filter: Rating (min & max)
        if (minRating.HasValue)
        {
            query = query.Where(l => l.Rating >= minRating.Value);
            _logger.Info($"Applied min rating filter: {minRating}");
        }

        if (maxRating.HasValue)
        {
            query = query.Where(l => l.Rating <= maxRating.Value);
            _logger.Info($"Applied max rating filter: {maxRating}");
        }

        // Filter: Distance (min & max)
        if (minDistance.HasValue)
        {
            query = query.Where(l => l.TotalDistance >= minDistance.Value);
            _logger.Info($"Applied min distance filter: {minDistance}");
        }

        if (maxDistance.HasValue)
        {
            query = query.Where(l => l.TotalDistance <= maxDistance.Value);
            _logger.Info($"Applied max distance filter: {maxDistance}");
        }

        // Filter: Time (min & max, in Minuten)
        if (minTime.HasValue)
        {
            query = query.Where(l => l.TotalTime >= minTime.Value);
            _logger.Info($"Applied min time filter: {minTime}");
        }

        if (maxTime.HasValue)
        {
            query = query.Where(l => l.TotalTime <= maxTime.Value);
            _logger.Info($"Applied max time filter: {maxTime}");
        }

        // Get all logs
        var logs = await query
            .Include(l => l.Tour)
            .OrderByDescending(l => l.DateTime)
            .ToListAsync();

        _logger.Info($"Search returned {logs.Count} logs");

        var result = new SearchLogResultDto
        {
            TotalCount = logs.Count,
            Logs = logs.Select(l => new SearchLogDto
            {
                LogId = l.LogId,
                TourId = l.TourId,
                TourName = l.Tour?.Name ?? "Unknown",
                DateTime = l.DateTime,
                Comment = l.Comment ?? "",
                Difficulty = l.Difficulty,
                TotalDistance = l.TotalDistance,
                TotalTime = l.TotalTime,
                Rating = l.Rating
            }).ToList()
        };

        return result;
    }
}