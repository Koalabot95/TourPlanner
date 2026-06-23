using backend.DTOs;

namespace backend.Interfaces;

public interface ITourService
{
    // GetAllToursAsync - Alle Tours des Users (leer = [], nicht 404)
    Task<(IEnumerable<TourDto>? Tours, string? ErrorField, string? ErrorMessage, int StatusCode)> GetAllToursAsync(string userId);

    // GetTourByIdAsync - Einzelne Tour mit Ownership-Check
    Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> GetTourByIdAsync(Guid id, string userId);

    // CreateTourAsync - Neue Tour mit OpenRouteService Integration
    Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> CreateTourAsync(TourDto dto, string userId);

    // UpdateTourAsync - Tour aktualisieren mit Ownership-Check
    Task<(TourDto? Tour, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateTourAsync(Guid id, TourDto dto, string userId);

    // DeleteTourAsync - Tour + Cascade (Logs, Images) + MapSnapshot-Datei löschen
    Task<(bool Success, string? ErrorField, string? ErrorMessage, int StatusCode)> DeleteTourAsync(Guid id, string userId);
}