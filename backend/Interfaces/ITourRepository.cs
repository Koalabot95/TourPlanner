using System;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Interfaces
{
    public interface ITourRepository
    {
        // Holt eine Tour anhand ihrer Guid, um den Besitzer (UserId) zu prüfen
        Task<Tour?> GetByIdAsync(Guid id);

        // READ: Holt alle Touren aus der Datenbank
        Task<IEnumerable<Tour>> GetAllAsync();

        // CREATE
        Task<Tour> CreateAsync(Tour tour);

        // UPDATE - / Aktualisiert die Tour in der Datenbank nach der Neuberechnung von Beliebtheit und Kinderfreundlichkeit
        Task UpdateAsync(Tour tour);

        // DELETE
        Task DeleteAsync(Tour tour);

    }
}