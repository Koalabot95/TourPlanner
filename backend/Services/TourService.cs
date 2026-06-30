using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;

namespace backend.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IOpenRouteServiceClient _routeClient;
    private readonly IImageService _imageService;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(TourService));

    public TourService(ITourRepository tourRepository, IOpenRouteServiceClient routeClient, IImageService imageService)
    {
        _tourRepository = tourRepository;
        _routeClient = routeClient;
        _imageService = imageService;
    }

    // READ: Holt alle Tours des Users
    public async Task<(IEnumerable<TourDto>? Tours, string? ErrorField, string? ErrorMessage, int StatusCode)> GetAllToursAsync(string userId)
    {
        _logger.Info($"Versuch, alle Tours für User {userId} zu laden.");

        try
        {
            var allTours = await _tourRepository.GetAllAsync();

            // Filtere nur Tours des Users
            var userTours = allTours
                .Where(t => t.UserId.ToString() == userId)
                .ToList();

            var dtos = userTours.Select(MapToDto).ToList();

            _logger.Info($"Erfolgreich {dtos.Count} Tours für User {userId} geladen.");
            return (dtos, null, null, 200);
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler beim Laden der Tours für User {userId}", ex);
            return (null, "error", "Ein Fehler ist beim Laden der Tours aufgetreten.", 500);
        }
    }

    // READ: Holt eine spezifische Tour nach ID mit Ownership-Check
    public async Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> GetTourByIdAsync(Guid id, string userId)
    {
        _logger.Info($"Versuch, Tour {id} für User {userId} zu laden.");

        var tour = await _tourRepository.GetByIdAsync(id);

        if (tour == null)
        {
            _logger.Warn($"Tour {id} existiert nicht.");
            return (null, "tourId", "Tour existiert nicht.", 404);
        }

        // Ownership-Check: Darf dieser User diese Tour sehen?
        if (tour.UserId.ToString() != userId)
        {
            _logger.Warn($"User {userId} versucht auf fremde Tour {id} zuzugreifen.");
            return (null, "auth", "Keine Berechtigung für diese Tour.", 403);
        }

        _logger.Info($"Tour {id} erfolgreich für User {userId} geladen.");
        return (MapToDto(tour), null, null, 200);
    }



    // CREATE: Erstellt eine neue Tour mit OpenRouteService Integration
    public async Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> CreateTourAsync(TourDto dto, string userId)
    {
        _logger.Info($"Versuch, neue Tour für User {userId} zu erstellen.");

        // Validiere das DTO
        var validation = ValidateTourDto(dto);
        if (!validation.IsValid)
        {
            _logger.Warn($"Validierungsfehler bei Tour-Erstellung: {validation.Message}");
            return (null, validation.Field, validation.Message, 400);
        }

        // Erstelle Tour-Model
        var tour = new Tour
        {
            TourId = Guid.NewGuid(),
            UserId = Guid.Parse(userId),
            Name = dto.Name,
            Description = dto.Description,
            StartLocation = dto.StartLocation,
            EndLocation = dto.EndLocation,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TransportType = dto.TransportType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Popularity = 0,
            ChildFriendliness = 0.0
        };

        // Rufe OpenRouteService auf um Distance & EstimatedTime zu berechnen
        _logger.Info($"Rufe OpenRouteService auf für {dto.StartLocation} → {dto.EndLocation}");
        var routeInfo = await _routeClient.GetRouteAsync(
            dto.StartLocation,
            dto.EndLocation,
            dto.TransportType
        );

        if (routeInfo != null)
        {
            tour.Distance = routeInfo.Distance;
            tour.EstimatedTime = routeInfo.EstimatedTime;
            _logger.Info($"OpenRouteService erfolgreich: {tour.Distance}km, {tour.EstimatedTime}h");
        }
        else
        {
            _logger.Warn("OpenRouteService nicht verfügbar - Distance & EstimatedTime bleiben null");
        }

        // Speichere in DB
        var createdTour = await _tourRepository.CreateAsync(tour);

        _logger.Info($"Tour {createdTour.TourId} erfolgreich erstellt für User {userId}");
        return (MapToDto(createdTour), null, null, 201);
    }

    // UPDATE: Aktualisiert eine Tour mit Ownership-Check
    public async Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateTourAsync(Guid id, TourDto dto, string userId)
    {
        _logger.Info($"Versuch, Tour {id} für User {userId} zu aktualisieren.");

        // Hole Tour
        var tour = await _tourRepository.GetByIdAsync(id);
        if (tour == null)
        {
            _logger.Warn($"Tour {id} existiert nicht.");
            return (null, "tourId", "Tour existiert nicht.", 404);
        }

        // Ownership-Check: Darf dieser User diese Tour ändern?
        if (tour.UserId.ToString() != userId)
        {
            _logger.Warn($"User {userId} versucht fremde Tour {id} zu aktualisieren.");
            return (null, "auth", "Keine Berechtigung für diese Tour.", 403);
        }

        // Validiere das DTO
        var validation = ValidateTourDto(dto);
        if (!validation.IsValid)
        {
            _logger.Warn($"Validierungsfehler bei Update: {validation.Message}");
            return (null, validation.Field, validation.Message, 400);
        }

        // Prüfe ob Route sich geändert hat (dann neu berechnen)
        bool routeChanged =
            tour.StartLocation != dto.StartLocation ||
            tour.EndLocation != dto.EndLocation ||
            tour.TransportType != dto.TransportType;

        // Aktualisiere Felder
        tour.Name = dto.Name;
        tour.Description = dto.Description;
        tour.StartLocation = dto.StartLocation;
        tour.EndLocation = dto.EndLocation;
        tour.StartDate = dto.StartDate;
        tour.EndDate = dto.EndDate;
        tour.TransportType = dto.TransportType;
        tour.UpdatedAt = DateTime.UtcNow;

        // Wenn Route sich geändert hat: Rufe OpenRouteService neu auf
        if (routeChanged)
        {
            _logger.Info("Route hat sich geändert - berechne neu");
            var routeInfo = await _routeClient.GetRouteAsync(
                dto.StartLocation,
                dto.EndLocation,
                dto.TransportType
            );

            if (routeInfo != null)
            {
                tour.Distance = routeInfo.Distance;
                tour.EstimatedTime = routeInfo.EstimatedTime;
                _logger.Info($"Neue Route: {tour.Distance}km, {tour.EstimatedTime}h");
            }
            else
            {
                tour.Distance = null;
                tour.EstimatedTime = null;
                _logger.Warn("OpenRouteService nicht verfügbar");
            }
        }

        // Speichere Update in DB
        await _tourRepository.UpdateAsync(tour);

        _logger.Info($"Tour {id} erfolgreich aktualisiert für User {userId}");
        return (MapToDto(tour), null, null, 200);
    }

    // DELETE: Löscht eine Tour mit allen TourLogs & Images (Cascade) + MapSnapshot-Datei
    public async Task<(bool Success, string? ErrorField, string? ErrorMessage, int StatusCode)> DeleteTourAsync(Guid id, string userId)
    {
        _logger.Info($"Versuch, Tour {id} für User {userId} zu löschen.");

        // Hole Tour
        var tour = await _tourRepository.GetByIdAsync(id);
        if (tour == null)
        {
            _logger.Warn($"Tour {id} existiert nicht.");
            return (false, "tourId", "Tour existiert nicht.", 404);
        }

        // Ownership-Check: Darf dieser User diese Tour löschen?
        if (tour.UserId.ToString() != userId)
        {
            _logger.Warn($"User {userId} versucht fremde Tour {id} zu löschen.");
            return (false, "auth", "Keine Berechtigung für diese Tour.", 403);
        }

        // Lösche MapSnapshot-Datei vom Filesystem falls vorhanden
        if (!string.IsNullOrEmpty(tour.MapSnapshotPath))
        {
            try
            {
                _imageService.DeleteImage(tour.MapSnapshotPath);
                _logger.Info($"MapSnapshot-Datei gelöscht: {tour.MapSnapshotPath}");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Fehler beim Löschen der MapSnapshot-Datei", ex);
            }
        }

        // Lösche Tour (TourLogs und Images werden automatisch gelöscht via Cascade)
        await _tourRepository.DeleteAsync(tour);

        _logger.Info($"Tour {id} erfolgreich gelöscht für User {userId}");
        return (true, null, null, 204);
    }

    // PRIVATE HELPER METHODS 

    // Validiert ein TourDto
    private (bool IsValid, string? Field, string? Message) ValidateTourDto(TourDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "name", "Der Name ist erforderlich.");

        if (dto.Name.Length > 100)
            return (false, "name", "Der Name darf maximal 100 Zeichen lang sein.");

        if (string.IsNullOrWhiteSpace(dto.StartLocation))
            return (false, "startLocation", "Der Startort ist erforderlich.");

        if (string.IsNullOrWhiteSpace(dto.EndLocation))
            return (false, "endLocation", "Der Zielort ist erforderlich.");

        // StartDate muss vor EndDate liegen
        if (dto.StartDate > dto.EndDate)
            return (false, "startDate", "Das Startdatum muss vor dem Enddatum liegen.");

        // TransportType validieren
        if (!Enum.IsDefined(typeof(TransportMode), dto.TransportType))
            return (false, "transportType", "Der Transporttyp ist ungültig. Erlaubt: Walking, Cycling, Driving.");

        return (true, null, null);
    }

    // Mapped Tour-Model zu TourDto
    private TourDto MapToDto(Tour tour)
    {
        return new TourDto
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
            Popularity = tour.Popularity,
            ChildFriendliness = tour.ChildFriendliness,
            MapSnapshotPath = tour.MapSnapshotPath
        };
    }
}