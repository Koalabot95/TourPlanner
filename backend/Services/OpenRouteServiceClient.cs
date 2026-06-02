using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using backend.Models; // für TransportMode
using log4net;
using Microsoft.Extensions.Configuration;

namespace backend.Services;


public class RouteResult
{
    public double Distance { get; set; }      // km
    public double EstimatedTime { get; set; } // Stunden
}

public class OpenRouteServiceClient : IOpenRouteServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private static readonly ILog _log = LogManager.GetLogger(typeof(OpenRouteServiceClient));

    public OpenRouteServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouteService:ApiKey"]!;

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _log.Fatal("OpenRouteService API Key is missing in configuration!");
            throw new InvalidOperationException("Startup Exception: OpenRouteService API Key is missing in appsettings.json.");
        }
    }

    //wird von TourService aufgerufen, um die Route zu berechnen
    public async Task<RouteResult?> GetRouteAsync(string start, string end, TransportMode type)
    {
        //Mapping des TransportMode 
        string profile = type switch
        {
            TransportMode.Walking => "foot-walking",
            TransportMode.Cycling => "cycling-regular",
            TransportMode.Driving => "driving-car",
            _ => "driving-car"
        };

        _log.Info($"Sending ORS routing request from '{start}' to '{end}' using profile '{profile}'");

        //HttpRequest an OpenRouteService API senden und Fehlerbehandlung
        try
        {
            string url = $"v2/directions/{profile}?api_key={_apiKey}&start={start}&end={end}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _log.Error($"OpenRouteService API returned error status: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<OrsResponse>();
            if (data?.Features == null || data.Features.Length == 0)
            {
                _log.Warn("OpenRouteService returned an empty route result.");
                return null;
            }

            var summary = data.Features[0].Properties.Summary;

            // Konvertierung zu double 
            var result = new RouteResult
            {
                Distance = summary.Distance / 1000.0,
                EstimatedTime = summary.Duration / 3600.0
            };

            _log.Info($"Successfully calculated route: {result.Distance} km, {result.EstimatedTime} hours.");
            return result;
        }
        catch (HttpRequestException ex)
        {
            _log.Error("Network error occurred while connecting to OpenRouteService API.", ex);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _log.Error("OpenRouteService API request timed out.", ex);
            return null;
        }
        catch (Exception ex)
        {
            _log.Error("An unexpected error occurred during OpenRouteService calculation.", ex);
            return null;
        }
    }

    private class OrsResponse
    {
        [JsonPropertyName("features")]
        public OrsFeature[] Features { get; set; } = Array.Empty<OrsFeature>();
    }

    private class OrsFeature
    {
        [JsonPropertyName("properties")]
        public OrsProperties Properties { get; set; } = new();
    }

    private class OrsProperties
    {
        [JsonPropertyName("summary")]
        public OrsSummary Summary { get; set; } = new();
    }

    private class OrsSummary
    {
        [JsonPropertyName("distance")]
        public double Distance { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }
    }
}