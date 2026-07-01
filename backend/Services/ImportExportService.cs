using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ImportExportService : IImportExportService
{
    private readonly TourPlannerContext _context;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(ImportExportService));

    public ImportExportService(TourPlannerContext context)
    {
        _context = context;
    }

    public async Task<string> ExportToursAsJsonAsync(string userId)
    {
        _logger.Info($"Exporting tours for user {userId}");

        var userIdGuid = Guid.Parse(userId);
        var tours = await _context.Tours
            .Where(t => t.UserId == userIdGuid)
            .Include(t => t.TourLogs)
            .ToListAsync();

        var exportData = tours.Select(t => new
        {
            t.Name,
            t.Description,
            t.StartLocation,
            t.EndLocation,
            t.StartDate,
            t.EndDate,
            t.TransportType,
            t.Distance,
            t.EstimatedTime,
            t.RouteInformation,
            Logs = t.TourLogs.Select(l => new
            {
                l.DateTime,
                l.Comment,
                l.Difficulty,
                l.TotalDistance,
                l.TotalTime,
                l.Rating
            }).ToList()
        }).ToList();

        var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        _logger.Info($"Exported {tours.Count} tours for user {userId}");

        return json;
    }

    public async Task<(bool Success, string? ErrorMessage, List<TourDto>? ImportedTours)> ImportToursFromJsonAsync(string userId, string jsonContent)
    {
        _logger.Info($"Importing tours for user {userId}");

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var toursData = JsonSerializer.Deserialize<List<dynamic>>(jsonContent, options);

            if (toursData == null || toursData.Count == 0)
            {
                _logger.Warn("Import JSON is empty or invalid");
                return (false, "JSON is empty or invalid", null);
            }

            var userIdGuid = Guid.Parse(userId);
            var importedTours = new List<TourDto>();

            foreach (var tourData in toursData)
            {
                try
                {
                    // Parse Tour
                    var tourJson = JsonSerializer.Serialize(tourData);
                    var tourDto = JsonSerializer.Deserialize<TourImportDto>(tourJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (tourDto != null)
                    {
                        // Erstelle Tour Model
                        var tour = new Tour
                        {
                            TourId = Guid.NewGuid(),
                            UserId = userIdGuid,
                            Name = tourDto.Name,
                            Description = tourDto.Description,
                            StartLocation = tourDto.StartLocation,
                            EndLocation = tourDto.EndLocation,
                            StartDate = tourDto.StartDate,
                            EndDate = tourDto.EndDate,
                            TransportType = tourDto.TransportType,
                            Distance = tourDto.Distance,
                            EstimatedTime = tourDto.EstimatedTime,
                            RouteInformation = tourDto.RouteInformation,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Popularity = 0,
                            ChildFriendliness = 0.0,
                            TourLogs = new List<TourLog>()
                        };

                        // Importiere TourLogs falls vorhanden
                        if (tourDto.Logs != null && tourDto.Logs.Any())
                        {
                            foreach (var logData in tourDto.Logs)
                            {
                                var log = new TourLog
                                {
                                    LogId = Guid.NewGuid(),
                                    TourId = tour.TourId,
                                    Name = $"Log from {logData.DateTime:yyyy-MM-dd}",
                                    DateTime = logData.DateTime,
                                    Comment = logData.Comment ?? "",
                                    Difficulty = logData.Difficulty,
                                    TotalDistance = logData.TotalDistance,
                                    TotalTime = logData.TotalTime,
                                    Rating = logData.Rating,
                                    CreatedAt = DateTime.UtcNow
                                };
                                tour.TourLogs.Add(log);
                            }
                        }

                        // Speichere Tour + Logs
                        _context.Tours.Add(tour);
                        await _context.SaveChangesAsync();

                        // Berechne Popularity + ChildFriendliness
                        if (tour.TourLogs.Any())
                        {
                            tour.Popularity = tour.TourLogs.Count;

                            double avgDifficultyScore = tour.TourLogs.Average(l => l.Difficulty switch
                            {
                                "Easy" => 3.0,
                                "Moderate" => 3.0,
                                "Challenging" => 2.0,
                                "Hard" => 1.0,
                                _ => 2.0
                            });

                            double avgDistanceScore = tour.TourLogs.Average(l => l.TotalDistance switch
                            {
                                < 5 => 4.0,
                                < 15 => 3.0,
                                < 30 => 2.0,
                                _ => 1.0
                            });

                            double avgTimeScore = tour.TourLogs.Average(l => l.TotalTime switch
                            {
                                < 60 => 4.0,
                                < 180 => 3.0,
                                < 360 => 2.0,
                                _ => 1.0
                            });

                            double finalScore = (avgDifficultyScore + avgDistanceScore + avgTimeScore) / 3.0;
                            tour.ChildFriendliness = Math.Round(finalScore, 1);

                            _context.Tours.Update(tour);
                            await _context.SaveChangesAsync();
                        }

                        importedTours.Add(new TourDto
                        {
                            TourId = tour.TourId,
                            Name = tour.Name,
                            Description = tour.Description,
                            StartLocation = tour.StartLocation,
                            EndLocation = tour.EndLocation,
                            StartDate = tour.StartDate,
                            EndDate = tour.EndDate,
                            TransportType = tour.TransportType,
                            Distance = tour.Distance,
                            EstimatedTime = tour.EstimatedTime,
                            RouteInformation = tour.RouteInformation,
                            Popularity = tour.Popularity,
                            ChildFriendliness = tour.ChildFriendliness
                        });

                        _logger.Info($"Imported tour '{tour.Name}' with {tour.TourLogs.Count} logs");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Failed to import individual tour: {ex.Message}");
                    continue;
                }
            }

            _logger.Info($"Successfully imported {importedTours.Count} tours for user {userId}");
            return (true, null, importedTours);
        }
        catch (JsonException ex)
        {
            _logger.Error($"JSON parsing error: {ex.Message}");
            return (false, $"Invalid JSON format: {ex.Message}", null);
        }
        catch (Exception ex)
        {
            _logger.Error($"Import error: {ex.Message}");
            return (false, $"Import failed: {ex.Message}", null);
        }
    }
}