using System.Threading.Tasks;
using backend.Models;
using backend.Services; 

namespace backend.Interfaces;

public interface IOpenRouteServiceClient
{
    Task<RouteResult?> GetRouteAsync(string start, string end, TransportMode type);
}