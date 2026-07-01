using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class FavoriteService : IFavoriteService
{
    private readonly TourPlannerContext _context;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(FavoriteService));

    public FavoriteService(TourPlannerContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string? ErrorMessage, int StatusCode)> AddFavoriteAsync(Guid tourId, string userId)
    {
        _logger.Info($"Adding favorite tour {tourId} for user {userId}");

        var userIdGuid = Guid.Parse(userId);

        // Check if tour exists and belongs to user or is public
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.TourId == tourId);
        if (tour == null)
        {
            _logger.Warn($"Tour {tourId} not found");
            return (false, "Tour does not exist", 404);
        }

        // Check if already favorite
        var existingFavorite = await _context.FavoriteTours
            .FirstOrDefaultAsync(f => f.TourId == tourId && f.UserId == userIdGuid);

        if (existingFavorite != null)
        {
            _logger.Warn($"Tour {tourId} is already favorite for user {userId}");
            return (false, "Tour is already in favorites", 400);
        }

        var favorite = new FavoriteTour
        {
            FavoriteTourId = Guid.NewGuid(),
            UserId = userIdGuid,
            TourId = tourId,
            CreatedAt = DateTime.UtcNow
        };

        _context.FavoriteTours.Add(favorite);
        await _context.SaveChangesAsync();

        _logger.Info($"Successfully added favorite tour {tourId} for user {userId}");
        return (true, null, 201);
    }

    public async Task<(bool Success, string? ErrorMessage, int StatusCode)> RemoveFavoriteAsync(Guid tourId, string userId)
    {
        _logger.Info($"Removing favorite tour {tourId} for user {userId}");

        var userIdGuid = Guid.Parse(userId);

        var favorite = await _context.FavoriteTours
            .FirstOrDefaultAsync(f => f.TourId == tourId && f.UserId == userIdGuid);

        if (favorite == null)
        {
            _logger.Warn($"Favorite tour {tourId} not found for user {userId}");
            return (false, "Tour is not in favorites", 404);
        }

        _context.FavoriteTours.Remove(favorite);
        await _context.SaveChangesAsync();

        _logger.Info($"Successfully removed favorite tour {tourId} for user {userId}");
        return (true, null, 204);
    }

    public async Task<(List<FavoriteTourDto>? Tours, string? ErrorMessage, int StatusCode)> GetFavoritesToursAsync(string userId)
    {
        _logger.Info($"Getting favorite tours for user {userId}");

        var userIdGuid = Guid.Parse(userId);

        var favorites = await _context.FavoriteTours
            .Where(f => f.UserId == userIdGuid)
            .Include(f => f.Tour)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        var dtos = favorites.Select(f => new FavoriteTourDto
        {
            FavoriteTourId = f.FavoriteTourId,
            TourId = f.Tour!.TourId,
            Name = f.Tour.Name,
            Description = f.Tour.Description,
            StartLocation = f.Tour.StartLocation,
            EndLocation = f.Tour.EndLocation,
            Distance = (decimal)(f.Tour.Distance ?? 0),
            EstimatedTime = (int)(f.Tour.EstimatedTime ?? 0),
            TransportMode = f.Tour.TransportType.ToString(),
            Popularity = f.Tour.Popularity,
            ChildFriendliness = (decimal)f.Tour.ChildFriendliness,
            AddedAt = f.CreatedAt
        }).ToList();

        _logger.Info($"Retrieved {dtos.Count} favorite tours for user {userId}");
        return (dtos, null, 200);
    }

    public async Task<(bool IsFavorite, int StatusCode)> IsFavoriteTourAsync(Guid tourId, string userId)
    {
        var userIdGuid = Guid.Parse(userId);

        var isFavorite = await _context.FavoriteTours
            .AnyAsync(f => f.TourId == tourId && f.UserId == userIdGuid);

        return (isFavorite, 200);
    }
}