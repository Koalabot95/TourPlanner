using System;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Interfaces
{
    public interface ITourRepository
    {
        // Holt eine Tour anhand ihrer Guid, um den Besitzer (UserId) zu prüfen
        Task<Tour?> GetByIdAsync(Guid id);

        // Aktualisiert die Tour in der Datenbank nach der Neuberechnung von Beliebtheit und Kinderfreundlichkeit
        Task UpdateAsync(Tour tour);
    }
}