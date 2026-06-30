using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using backend.Models; // für TransportMode
using backend.Interfaces;
using log4net;
using Microsoft.Extensions.Configuration;

namespace backend.Services;


public class RouteResult
{
    public double Distance { get; set; }      // km
    public double EstimatedTime { get; set; } // Stunden
    public string? GeometryGeoJson { get; set; }
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
        //Adresse in Koordinaten umwandeln
        string? startCoords = await GeocodeAddressAsync(start);
        string? endCoords = await GeocodeAddressAsync(end);

        if (string.IsNullOrEmpty(startCoords) || string.IsNullOrEmpty(endCoords))
        {
            _log.Warn($"Routing abgebrochen: Koordinaten für Start '{start}' oder Ende '{end}' konnten nicht aufgelöst werden.");
            return null;
        }

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
            string url = $"v2/directions/{profile}?api_key={_apiKey}&start={startCoords}&end={endCoords}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _log.Error($"OpenRouteService API returned error status: {response.StatusCode} - {response.ReasonPhrase}. Body: {errorBody}");
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<OrsResponse>();

            if (data?.Features == null || data.Features.Length == 0)
            {
                _log.Warn("OpenRouteService returned an empty route result.");
                return null;
            }

            var summary = data.Features[0].Properties.Summary;

            // ORS gibt die Geometrie als GeoJSON zurück
            var geometry = data.Features[0].Geometry;
            var geoJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                type = geometry.Type,
                coordinates = geometry.Coordinates
            });

            // Konvertierung zu double 
            var result = new RouteResult
            {
                Distance = Math.Round(summary.Distance / 1000.0, 1),
                EstimatedTime = Math.Round(summary.Duration / 3600.0, 1),
                GeometryGeoJson = geoJson
            };

            _log.Info($"Successfully calculated route: {result.Distance} km, {result.EstimatedTime} hours.");
            return result;
        }
        catch (Exception ex)
        {
            _log.Error("An unexpected error occurred during OpenRouteService calculation.", ex);
            return null;
        }
    }

    private async Task<string?> GeocodeAddressAsync(string address)
    {
        try
        {
            // ORS Geocoding Endpoint aufrufen
            // Absolute URL mit /geocode/ statt /v1/geocode/ verwenden
            string url = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={Uri.EscapeDataString(address)}&size=1";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            var data = await response.Content.ReadFromJsonAsync<GeocodeResponse>();
            if (data?.Features == null || data.Features.Length == 0) return null;

            // ORS gibt Koordinaten als [lng, lat] zurück
            var coords = data.Features[0].Geometry.Coordinates;
            return $"{coords[0].ToString(System.Globalization.CultureInfo.InvariantCulture)},{coords[1].ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        }
        catch (Exception ex)
        {
            _log.Error($"Geocoding failed for {address}", ex);
            return null;
        }
    }

    private class OrsResponse
    {
        [JsonPropertyName("features")] public OrsFeature[] Features { get; set; } = Array.Empty<OrsFeature>();
    }
    private class OrsFeature
    {
        [JsonPropertyName("properties")] public OrsProperties Properties { get; set; } = new();
        [JsonPropertyName("geometry")] public OrsGeometry Geometry { get; set; } = new(); // NEU
    }

    private class OrsGeometry
    {
        [JsonPropertyName("coordinates")] public double[][] Coordinates { get; set; } = Array.Empty<double[]>();
        [JsonPropertyName("type")] public string Type { get; set; } = "LineString";
    }
    private class OrsProperties
    {
        [JsonPropertyName("summary")] public OrsSummary Summary { get; set; } = new();
    }
    private class OrsSummary
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
    }

    private class GeocodeResponse
    {
        [JsonPropertyName("features")] public GeocodeFeature[] Features { get; set; } = Array.Empty<GeocodeFeature>();
    }
    private class GeocodeFeature
    {
        [JsonPropertyName("geometry")] public GeocodeGeometry Geometry { get; set; } = new();
    }
    private class GeocodeGeometry
    {
        [JsonPropertyName("coordinates")] public double[] Coordinates { get; set; } = Array.Empty<double>();
    }
}