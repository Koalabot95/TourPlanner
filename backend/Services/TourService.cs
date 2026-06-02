using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public class TourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IOpenRouteServiceClient _routeClient;

    public TourService(ITourRepository tourRepository, IOpenRouteServiceClient routeClient)
    {
        _tourRepository = tourRepository;
        _routeClient = routeClient;
    }

    public async Task<Tour> CreateTourAsync(Tour tour)
    {
        //OpenRouteService aufrufen
        var routeInfo = await _routeClient.GetRouteAsync(tour.StartLocation, tour.EndLocation, tour.TransportType);

        if (routeInfo != null)
        {
            tour.Distance = routeInfo.Distance;
            tour.EstimatedTime = routeInfo.EstimatedTime;
        }

        return await _tourRepository.CreateAsync(tour);
    }
}