using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class TourLogRepository : ITourLogRepository
    {
        private readonly TourPlannerContext _context;

        public TourLogRepository(TourPlannerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TourLog>> GetLogsByTourIdAsync(Guid tourId)
        {
            return await _context.TourLogs
                .Where(log => log.TourId == tourId)
                .ToListAsync();
        }

        public async Task<TourLog?> GetByIdAsync(Guid id)
        {
            return await _context.TourLogs
                .Include(log => log.Tour)
                .FirstOrDefaultAsync(log => log.LogId == id);
        }

        public async Task<TourLog> CreateAsync(TourLog log)
        {
            await _context.TourLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task UpdateAsync(TourLog log)
        {
            _context.TourLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TourLog log)
        {
            _context.TourLogs.Remove(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TourLog>> GetAllAsync()
        {
            return await _context.TourLogs
                .Include(l => l.Tour)
                .ToListAsync();
        }
    }
}