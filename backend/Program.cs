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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "backend", Version = "v1" });

    // Definiert das Sicherheitskonzept (JWT Bearer) für Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization Header mit dem Bearer-Schema. Beispiel: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Macht das Schloss-Symbol an den Endpunkten sichtbar
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Add("Accept", "application/json, application/geo+json; charset=utf-8");
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

// 7. EF Core Migration -> sleep und retries hinzugefügt, falls die db länger braucht
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TourPlannerContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Datenbank ist noch nicht bereit. Warte... ({retries} Versuche übrig)");
            if (retries == 0)
            {
                Console.WriteLine("Datenbank-Migration fehlgeschlagen.");
                throw;
            }
            System.Threading.Thread.Sleep(5000); // 3 Sekunden warten vor dem nächsten Versuch
        }
    }
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

//Pfad zum Angular-Ausgabeordner definieren
var angularDistPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "browser");

var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(angularDistPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = fileProvider
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    ServeUnknownFileTypes = true 
});
app.UseRouting();
app.UseCors("AngularDevPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("browser/index.html");

app.Run();