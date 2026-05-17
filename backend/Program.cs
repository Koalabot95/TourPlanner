using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. PostgreSQL Zeitstempel
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 2. Datenbank-Anbindung
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TourPlannerContext>(options =>
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
builder.Services.AddSwaggerGen();

// 5. Auth Services
builder.Services.AddScoped<backend.Interfaces.IUserRepository, backend.Repositories.UserRepository>();
builder.Services.AddScoped<backend.Services.AuthService>();


// 6. log4net
var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly()!);
log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("Logging/log4net.config"));

var app = builder.Build();

// 7. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();   // Erzeugt die technische Beschreibung
    app.UseSwaggerUI();
} else
{
    app.UseHttpsRedirection();
}

// CORS anwenden (vor MapControllers)
app.UseCors("AllowAll");

// Für Login
app.UseAuthorization(); 

// 8. Controller-Routen aktivieren 
app.MapControllers(); 

app.Run();