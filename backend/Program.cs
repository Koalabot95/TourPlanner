using backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS -> erlaubt Anbindung ans Frontend
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
builder.Services.AddScoped<backend.Interfaces.IUserService, backend.Services.UserService>();

// repos: Tour- und Log-Datenbankbefehle auflösen 
builder.Services.AddScoped<backend.Interfaces.ITourLogRepository, backend.Repositories.TourLogRepository>();
builder.Services.AddScoped<backend.Interfaces.ITourRepository, backend.Repositories.TourRepository>();

// Services: Tour- und Log-Services registrieren
builder.Services.AddScoped<backend.Interfaces.ITourService, backend.Services.TourService>();
builder.Services.AddScoped<backend.Interfaces.ITourLogService, backend.Services.TourLogService>();

// Search Service registrieren
builder.Services.AddScoped<backend.Interfaces.ISearchService, backend.Services.SearchService>();

// Image Repository & Service registrieren
builder.Services.AddScoped<backend.Interfaces.IImageRepository, backend.Repositories.ImageRepository>();
builder.Services.AddScoped<backend.Interfaces.IImageService, backend.Services.ImageService>();

//OpenRouteServiceClient registrieren(Interface, Basis-URL und 10s Timeout)
builder.Services.AddHttpClient<backend.Interfaces.IOpenRouteServiceClient, backend.Services.OpenRouteServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openrouteservice.org/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT Secret is not configured in appsettings.json!");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// 6. log4net
var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly()!);
log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("Logging/log4net.config"));

var app = builder.Build();

// 7. EF Core Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TourPlannerContext>();
    dbContext.Database.Migrate();
}

// 8. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AngularDevPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();