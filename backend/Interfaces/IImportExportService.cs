using backend.DTOs;

namespace backend.Interfaces;

public interface IImportExportService
{
    Task<string> ExportToursAsJsonAsync(string userId);
    Task<(bool Success, string? ErrorMessage, List<TourDto>? ImportedTours)> ImportToursFromJsonAsync(string userId, string jsonContent);
}