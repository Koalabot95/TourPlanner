1. Überblick

Dieses Projekt implementiert eine Webapplikation, mit der es möglich ist, als Outdoor-Freund Touren zu planen, zu dokumentieren und auszuwerten.Die Anwendung ermöglicht das Erstellen, Verwalten und Analysieren von Touren und Logbüchern. Es ist möglich sich als Benutzer anzumelden, Touren zu planen, Logeinträge zu erstellen und Statistik dazu ausgeben zu lassen. Die Apllikation unterstützt vollständige CRUD-Operationen für Benutzer (Registrierung, Login mit Bearer-Token-Authentifizierung, Aktualisieren und Löschen von Benutzerdaten) sowie für Tour und Log-Einträge. Die Authentifizierung erfolgt über Bearer Tokens.

2. Technologie-Stack
IDE: Visual Studio Code mit C# Dev Kit Extension
Frontend: Typescript, Angular Framework
Backend: C#, ASP.Net Framework
OR-Mapper: Entity-Framework
Communication: HTTP
Serialization & Deserialization: JSON
Database: PostgreSQL
Logging: log4net
Testing: NUnit Framework

3.  Domain-Modelle

3.1 User
Enthält UserName, FullName, Email und PasswordHash (SHA256). Der UserName ist nach der Registrierung unveränderbar und dient als eindeutiger Identifikator.

3.2 Tour
Enthält TourId, UserName, Name, Description, StartLocation, EndLocation, TransportType, Distance, EstimatedTime, RouteInformation, Popularity und ChildFriendliness. Touren können Logs zugeordnet werden

3.3 TourLog
Ein TourLog repräsentiert einen Log-Eintrag einer Tour durch einen Benutzer. Jeder TourLog ist eindeutig einer Tour und einem Benutzer zugeordnet und enthält Datum, Bewertung (1–5), Schwierigkeit, Distanz, Zeit, Kommentar sowie einen Zeitstempel.
