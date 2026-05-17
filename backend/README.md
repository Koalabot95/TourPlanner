# TourPlanner - Projekt Setup & Dokumentation
Dieses Projekt ist ein Tourenplaner-System, bestehend aus einem .NET Web-API Backend und einem WPF Frontend. Die Datenhaltung erfolgt über eine PostgreSQL-Datenbank in Docker.

1. Datenbank & Infrastruktur
Stellen Sie sicher, dass Docker installiert ist. Navigieren Sie in das Hauptverzeichnis und starten Sie die Container:
```bash
docker-compose up -d

2. Ports & Login Data
Postgres: localhost:5432
pgAdmin: localhost:8080 (User: admin@admin.com, PW: admin)

2. Projekt starten
cd backend
dotnet restore
dotnet build
dotnet run

3. Konfiguration
Alle wichtigen Einstellungen (DB-Connection, API-Keys) befinden sich in der appsettings.json.

4. Architektur
Das Projekt folgt einer Layer-Architektur:

Controllers: API Endpunkte.
Services: Geschäftslogik (Berechnungen, Validierung).
Data: EF Core DB-Kontext.
Models: Entity Klassen.

5. Entwicklungsprozess
Aktuellen Stand holen: git pull origin main
Neuen Branch erstellen: git checkout -b feature/new_feature
Änderungen testen: dotnet build
Mergen: Erst in den main mergen, wenn der Code lokal mit dotnet build fehlerfrei baut.

6. Logging
Logs werden via log4net erfasst und im Verzeichnis backend/logs gespeichert.