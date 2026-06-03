using System;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using backend.Models;

namespace backend.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly TourPlannerContext _context;

        public ImageRepository(TourPlannerContext context)
        {
            _context = context;
        }

        public async Task<Image> CreateAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync(); // Schreibt den Pfad und die IDs in die DB
            return image;
        }

        public async Task<bool> DeleteAsync(Guid imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null) return false;

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}