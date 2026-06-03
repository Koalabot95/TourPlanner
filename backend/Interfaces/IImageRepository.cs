using System.Threading.Tasks;
using backend.Models;

namespace backend.Interfaces
{
    public interface IImageRepository
    {
        Task<Image> CreateAsync(Image image);
        Task<bool> DeleteAsync(Guid imageId);
    }
}