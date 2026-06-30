using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Interfaces
{
    public interface ITourLogRepository
    {
        Task<IEnumerable<TourLog>> GetLogsByTourIdAsync(Guid tourId);
        Task<TourLog?> GetByIdAsync(Guid id);
        Task<TourLog> CreateAsync(TourLog log);
        Task UpdateAsync(TourLog log);
        Task DeleteAsync(TourLog log);
        Task<IEnumerable<TourLog>> GetAllAsync();
    }
}