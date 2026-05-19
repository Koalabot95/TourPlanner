using System;
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

        public async Task<Tour?> GetByIdAsync(Guid id)
        {
            return await _context.Tours
                .FirstOrDefaultAsync(t => t.TourId == id);
        }

        public async Task UpdateAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }
    }
}