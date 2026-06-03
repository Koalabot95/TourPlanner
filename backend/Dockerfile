# 1. Build-Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiert die .csproj und stellt die NuGet-Pakete wieder her
COPY backend.csproj ./
RUN dotnet restore

# Kopiert den gesamten Rest und baut die Anwendung
COPY . .
RUN dotnet publish backend.csproj -c Release -o /app/publish

# 2. Runtime-Stage: Nutzt ASP.NET Core Runtime Image zum Ausführen
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Kopiert die fertigen Kompilate aus der Build-Stage
COPY --from=build /app/publish .

# Erstellt den Uploads-Ordner im Container-Dateisystem
RUN mkdir -p /app/Uploads

# Startet die Anwendung über backend.dll
ENTRYPOINT ["dotnet", "backend.dll"]