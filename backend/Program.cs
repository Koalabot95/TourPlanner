using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. PostgreSQL Zeitstempel
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 2. Datenbank-Anbindung
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TourContext>(options =>
    options.UseNpgsql(connectionString));

// 3. CORS - Erlaubt den Zugriff vom Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Controller & Enum-Konvertierung
builder.Services.AddControllers()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// OpenAPI für Swagger/Dokumentation
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 5. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
} else
{
    app.UseHttpsRedirection();
}

// CORS anwenden (vor MapControllers)
app.UseCors("AllowAll");

// Für Login
app.UseAuthorization(); 

// 6. Controller-Routen aktivieren 
app.MapControllers(); 

app.Run();