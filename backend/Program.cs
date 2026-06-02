using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS -> erlaubt Anbdinung ans Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. PostgreSQL Zeitstempel
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 3. Datenbank-Anbindung
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TourPlannerContext>(options =>
    options.UseNpgsql(connectionString));


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

// repos: Tour- und Log-Datenbankbefehle auflösen 
builder.Services.AddScoped<backend.Interfaces.ITourLogRepository, backend.Repositories.TourLogRepository>();
builder.Services.AddScoped<backend.Interfaces.ITourRepository, backend.Repositories.TourRepository>();

//OpenRouteServiceClient registrieren(Interface, Basis-URL und 10s Timeout)
builder.Services.AddHttpClient<backend.Interfaces.IOpenRouteServiceClient, backend.Services.OpenRouteServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openrouteservice.org/");
    client.Timeout = TimeSpan.FromSeconds(10); 
});


// 6. log4net
var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly()!);
log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("Logging/log4net.config"));

var app = builder.Build();

// 7. Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();   
    app.UseSwaggerUI();
} else
{
    app.UseHttpsRedirection();
}

// CORS-Middleware aktivieren
app.UseRouting();

// CORS anwenden (vor MapControllers)
app.UseCors("AngularDevPolicy");

// Autorisierung
app.UseAuthentication();

// Für Login
app.UseAuthorization(); 

// 8. Controller-Routen aktivieren 
app.MapControllers(); 

app.Run();