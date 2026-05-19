using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;

namespace backend.Services
{
    public class TourLogService : ITourLogService
    {
        private readonly ITourLogRepository _logRepository;
        private readonly ITourRepository _tourRepository;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TourLogService));

        private readonly List<string> _validDifficulties = new() { "Easy", "Moderate", "Challenging", "Hard" };

        public TourLogService(ITourLogRepository logRepository, ITourRepository tourRepository)
        {
            _logRepository = logRepository;
            _tourRepository = tourRepository;
        }

        public async Task<(IEnumerable<TourLogResponseDto>? Logs, string? ErrorField, string? ErrorMessage, int StatusCode)> GetLogsForTourAsync(Guid tourId, string userId)
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return (null, "tourId", "Tour existiert nicht.", 404);
            if (tour.UserId.ToString() != userId) return (null, "auth", "Keine Berechtigung für diese Tour.", 403);

            var logs = await _logRepository.GetLogsByTourIdAsync(tourId);
            var dtos = logs.Select(MapToResponseDto);
            return (dtos, null, null, 200);
        }

        public async Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> GetLogByIdAsync(Guid id, string userId)
        {
            var log = await _logRepository.GetByIdAsync(id);
            if (log == null) return (null, "id", "Tour-Log existiert nicht.", 404);
            if (log.Tour == null || log.Tour.UserId.ToString() != userId) return (null, "auth", "Keine Berechtigung für dieses Log.", 403);

            return (MapToResponseDto(log), null, null, 200);
        }

        public async Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> CreateLogAsync(Guid tourId, TourLogCreateUpdateDto dto, string userId)
        {
            _logger.Info($"Versuch, ein neues Log für Tour {tourId} zu erstellen.");

            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null)
            {
                _logger.Warn($"Erstellung fehlgeschlagen: Tour {tourId} nicht gefunden.");
                return (null, "tourId", "Tour existiert nicht.", 404);
            }
            if (tour.UserId.ToString() != userId)
            {
                _logger.Warn($"Erstellung verweigert: User {userId} ist nicht Besitzer von Tour {tourId}.");
                return (null, "auth", "Keine Berechtigung für diese Tour.", 403);
            }

            var validation = ValidateDto(dto);
            if (!validation.IsValid)
            {
                _logger.Warn($"Validierungsfehler bei Erstellung für Tour {tourId}: {validation.Message}");
                return (null, validation.Field, validation.Message, 400);
            }

            var log = new TourLog
            {
                TourId = tourId,
                Name = dto.Name,
                DateTime = dto.DateTime,
                Comment = dto.Comment,
                Difficulty = dto.Difficulty,
                TotalDistance = dto.TotalDistance,
                TotalTime = dto.TotalTime,
                CreatedAt = DateTime.UtcNow
            };

            var createdLog = await _logRepository.CreateAsync(log);
            await RecalculateTourStatsAsync(tourId);

            _logger.Info($"Log {createdLog.LogId} erfolgreich für Tour {tourId} angelegt.");
            return (MapToResponseDto(createdLog), null, null, 201);
        }

        public async Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateLogAsync(Guid id, TourLogCreateUpdateDto dto, string userId)
        {
            _logger.Info($"Versuch, das Log {id} zu aktualisieren.");

            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
            {
                _logger.Warn($"Update fehlgeschlagen: Log {id} existiert nicht.");
                return (null, "id", "Tour-Log existiert nicht.", 404);
            }
            if (log.Tour == null || log.Tour.UserId.ToString() != userId)
            {
                _logger.Warn($"Update verweigert: User {userId} besitzt das Log {id} nicht.");
                return (null, "auth", "Keine Berechtigung für dieses Log.", 403);
            }

            var validation = ValidateDto(dto);
            if (!validation.IsValid)
            {
                _logger.Warn($"Validierungsfehler bei Update von Log {id}: {validation.Message}");
                return (null, validation.Field, validation.Message, 400);
            }

            log.Name = dto.Name;
            log.DateTime = dto.DateTime;
            log.Comment = dto.Comment;
            log.Difficulty = dto.Difficulty;
            log.TotalDistance = dto.TotalDistance;
            log.TotalTime = dto.TotalTime;

            await _logRepository.UpdateAsync(log);
            await RecalculateTourStatsAsync(log.TourId);

            _logger.Info($"Log {id} erfolgreich aktualisiert.");
            return (MapToResponseDto(log), null, null, 200);
        }

        public async Task<(bool Success, string? ErrorField, string? ErrorMessage, int StatusCode)> DeleteLogAsync(Guid id, string userId)
        {
            _logger.Info($"Versuch, das Log {id} zu löschen.");

            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
            {
                _logger.Warn($"Löschen fehlgeschlagen: Log {id} existiert nicht.");
                return (false, "id", "Tour-Log existiert nicht.", 404);
            }
            if (log.Tour == null || log.Tour.UserId.ToString() != userId)
            {
                _logger.Warn($"Löschen verweigert: User {userId} besitzt das Log {id} nicht.");
                return (false, "auth", "Keine Berechtigung für dieses Log.", 403);
            }

            Guid tourId = log.TourId;
            await _logRepository.DeleteAsync(log);

            await RecalculateTourStatsAsync(tourId);

            _logger.Info($"Log {id} erfolgreich gelöscht.");
            return (true, null, null, 204);
        }

        // --- Validierungs- & Berechnungslogik ---

        private (bool IsValid, string? Field, string? Message) ValidateDto(TourLogCreateUpdateDto dto)
        {
            if (dto.DateTime > DateTime.UtcNow) return (false, "dateTime", "Das Datum darf nicht in der Zukunft liegen.");
            if (string.IsNullOrEmpty(dto.Difficulty) || !_validDifficulties.Contains(dto.Difficulty))
                return (false, "difficulty", "Ungültiger Schwierigkeitsgrad. Erlaubt: Easy, Moderate, Challenging, Hard.");
            if (dto.TotalDistance <= 0) return (false, "totalDistance", "Die Distanz muss größer als 0 sein.");
            if (dto.TotalTime <= 0) return (false, "totalTime", "Die Zeitdauer muss größer als 0 sein.");
            return (true, null, null);
        }

        private async Task RecalculateTourStatsAsync(Guid tourId)
        {
            var logs = (await _logRepository.GetLogsByTourIdAsync(tourId)).ToList();
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return;

            if (!logs.Any())
            {
                tour.Popularity = 0;
                tour.ChildFriendliness = 0.0;
                await _tourRepository.UpdateAsync(tour);
                return;
            }

            // 1. Popularity (Anzahl aller Logeinträge)
            tour.Popularity = logs.Count;

            // 2. Child-Friendliness (Berechnung über Metriken)
            double avgDifficultyScore = logs.Average(l => l.Difficulty switch
            {
                "Easy" => 3.0,
                "Moderate" => 3.0,
                "Challenging" => 2.0,
                "Hard" => 1.0,
                _ => 2.0
            });

            double avgDistanceScore = logs.Average(l => l.TotalDistance switch
            {
                < 5 => 4.0,
                < 15 => 3.0,
                < 30 => 2.0,
                _ => 1.0
            });

            double avgTimeScore = logs.Average(l => l.TotalTime switch
            {
                < 60 => 4.0,
                < 180 => 3.0,
                < 360 => 2.0,
                _ => 1.0
            });

            double finalScore = (avgDifficultyScore + avgDistanceScore + avgTimeScore) / 3.0;
            tour.ChildFriendliness = Math.Round(finalScore, 1);

            await _tourRepository.UpdateAsync(tour);
        }

        private TourLogResponseDto MapToResponseDto(TourLog log)
        {
            return new TourLogResponseDto
            {
                LogId = log.LogId,
                TourId = log.TourId,
                Name = log.Name,
                DateTime = log.DateTime,
                Comment = log.Comment,
                Difficulty = log.Difficulty,
                TotalDistance = log.TotalDistance,
                TotalTime = log.TotalTime,
                Rating = log.Rating,
                CreatedAt = log.CreatedAt
            };
        }
    }
}