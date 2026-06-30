using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly TourPlannerContext _context;

        public TourRepository(TourPlannerContext context)
        {
            _context = context;
        }

        // READ: Sucht bestimmte Tour 
        public async Task<Tour?> GetByIdAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.TourLogs) 
                .FirstOrDefaultAsync(t => t.TourId == id);
        }

        // READ: Holt alle Touren
        public async Task<IEnumerable<Tour>> GetAllAsync()
        {
            return await _context.Tours
                .Include(t => t.TourLogs) // 
                .ToListAsync();
        }

        // CREATE
        public async Task<Tour> CreateAsync(Tour tour)
        {
            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        // UPDATE: Aktualisiert die Daten einer bestehenden Tour
        public async Task UpdateAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(Tour tour)
        {
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
    }
}