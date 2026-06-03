1. Überblick

Dieses Projekt implementiert eine Webapplikation, mit der es möglich ist, als Outdoor-Freund Touren zu planen, zu dokumentieren und auszuwerten.Die Anwendung ermöglicht das Erstellen, Verwalten und Analysieren von Touren und Logbüchern. Es ist möglich sich als Benutzer anzumelden, Touren zu planen, Logeinträge zu erstellen und Statistik dazu ausgeben zu lassen. Die Apllikation unterstützt vollständige CRUD-Operationen für Benutzer (Registrierung, Login mit Bearer-Token-Authentifizierung, Aktualisieren und Löschen von Benutzerdaten) sowie für Tour und Log-Einträge. Die Authentifizierung erfolgt über Bearer Tokens.

2. Technologie-Stack
IDE: Visual Studio Code mit C# Dev Kit Extension/Visual Studio
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

4. Architektur

Die Applikation basiert auf einer entkoppelten Client-Server-Architektur, bei der das Frontend und das Backend als eigenständige Systeme agieren und über standardisierte Schnittstellen kommunizieren.

4.1 Frontend-Architektur (Angular)
Das Frontend nutzt die komponentenbasierte Architektur des Angular-Frameworks. Jede Ansicht (z. B. Tour-Erstellung, Dashboard) ist in eine eigenständige Komponente unterteilt, bestehend aus einem HTML-Template (View) und einer TypeScript-Klasse (ViewModel). Die Synchronisation der Daten zwischen der Benutzeroberfläche und der Programmlogik erfolgt über Two-Way Data Binding (`[(ngModel)]`). Dies entspricht dem MVVM-Muster (Model-View-ViewModel) und stellt sicher, dass Benutzereingaben ohne manuelles Event-Handling direkt im Datenmodell aktualisiert werden.

4.2 Backend-Architektur (.NET)
Das Backend ist als ASP.NET Core Web-API nach dem MVC-Muster (Model-View-Controller) aufgebaut. Die Controller nehmen die HTTP-Anfragen des Frontends entgegen, die Validierung der Daten erfolgt direkt auf Basis von DTOs (Data Transfer Objects) und die Business-Logik erstellt das passende Domänenobjekt. Anschließend führt das entsprechende Repository die Datenbankoperationen durch, während der Entity Framework (in TourPlannerContext) das finale Mapping auf die PostgreSQL-Datenbank übernimmt.

4.3 Kommunikation und Datentransfer
Die Kommunikation zwischen Client und Server erfolgt vollständig zustandslos über das HTTP-Protokoll. Das Frontend sendet asynchrone HTTP-Requests (GET, POST, PUT, DELETE), welche vom Backend verarbeitet werden. Der Datenaustausch erfolgt im standardisierten JSON-Format.

5. Benutzeroberfläche und Wireframes

Das UI-Design wurde vor der Implementierung mithilfe von Wireframes konzipiert, um eine intuitive User Experience zu gewährleisten. Die Anwendung ist responsive gestaltet.

5.1 Login-Maske
Das Wireframe der Login-Maske sieht eine zentrierte, reduzierte Card vor. Sie enthält Eingabefelder für Benutzername und Passwort sowie ein dynamisches Banner für Fehlermeldungen (z.B. bei falschen Anmeldedaten).

5.2 Dashboard 
Die Hauptansicht ist als Split-Layout konzipiert. Auf der linken Seite befindet sich die Listenansicht aller verfügbaren Touren mit kompakten Übersichtskarten (Cards) inklusive Bild-Metadaten. Auf der rechten Seite befinden sich die Logs.

6. Datenhaltung und Validierungskonzept

6.1 Client-seitige Speicherung und Authentifizierung
Nach erfolgreichem Login sendet das Backend ein via SHA256 verschlüsseltes JSON Web Token (JWT / Bearer Token) an das Frontend zurück. Dieses Token wird im `localStorage` des Browsers persistent gespeichert und bei jedem darauffolgenden API-Request automatisch im HTTP-Authorization-Header mitgeschickt. Im aktuellen Entwicklungsstand werden auch temporäre Frontend-Daten (wie neu angelegte Touren oder Base64-Bilddaten) im `localStorage` gehalten, um die vollständige Offline-Bedienbarkeit der UI zu demonstrieren.

6.2 Validierung und Absturzsicherheit
Um Anwendungsabstürze durch fehlerhafte oder unvollständige Benutzereingaben zu verhindern, implementiert die Applikation eine defensive Validierungsstrategie:
- Frontend-Validierung: Formulare (z.B. bei der Tour-Erstellung) verwenden Angular-Template-Validierungen (wie das `required`-Attribut). Der "Speichern"-Button bleibt für den Benutzer deaktiviert, solange Pflichtfelder (wie der Tour-Name) leer sind. Zusätzlich werden visuelle Fehlermeldungen unter den Feldern eingeblendet.
- Fehler-Abfangung: Sollte ein Request fehlerhaft sein oder das Backend einen Fehlercode (z. B. 400 Bad Request bei falschen Login-Daten) zurückliefern, fängt das Frontend diesen Zustand über den `error`-Block ab. Die Fehlermeldung wird in eine UI-Variable geschrieben und im Error-Banner ausgegeben, anstatt dass die Applikation abstürzt.


7. CRUD-Operationen und Datenbeziehungen

Die Anwendung bildet ein relationales Datenmodell ab, welches über Entity Framework auf die PostgreSQL-Datenbank abgebildet wird. Es besteht eine 1:N-Beziehung zwischen einem Benutzer und seinen Touren, sowie eine 1:N-Beziehung zwischen einer Tour und den dazugehörigen Tour-Logs (eine Tour kann beliebig viele Log-Einträge besitzen).

Die UI unterstützt die Verwaltung dieser Daten über entsprechende Steuerelemente:
- Create: Formulare zur Erfassung neuer Touren (inklusive Datei-Upload für Bilder) und Logs.
- Read: Detailansichten der Touren/Tourlogs.
- Delete / Update: Buttons innerhalb der Benutzeroberfläche ermöglichen das Entfernen von Einträgen aus dem Datenbestand, wobei beim Löschen einer Tour automatisch alle verknüpften Tour-Logs mitgelöscht werden, um Dateninkonsistenzen zu vermeiden.
