using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Interfaces
{
	public interface ITourLogService
	{
		Task<(IEnumerable<TourLogResponseDto>? Logs, string? ErrorField, string? ErrorMessage, int StatusCode)> GetLogsForTourAsync(Guid tourId, string userId);
		Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> GetLogByIdAsync(Guid id, string userId);
		Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> CreateLogAsync(Guid tourId, TourLogCreateUpdateDto dto, string userId);
		Task<(TourLogResponseDto? Log, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateLogAsync(Guid id, TourLogCreateUpdateDto dto, string userId);
		Task<(bool Success, string? ErrorField, string? ErrorMessage, int StatusCode)> DeleteLogAsync(Guid id, string userId);
        Task<(IEnumerable<TourLogResponseDto> Logs, int StatusCode)> GetAllLogsAsync(string userId);
    }
}