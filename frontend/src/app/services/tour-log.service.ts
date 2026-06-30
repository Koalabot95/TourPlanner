import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TourLog, TourLogBackendDto } from '../models/tour-log.model';

@Injectable({
  providedIn: 'root'
})
export class TourLogService {
  private apiUrl = '/api/logs';

  constructor(private http: HttpClient) { }

  // GET: /api/logs/tour/{tourId}
  // Alle Logs für eine ganz bestimmte Tour laden
  getLogsForTour(tourId: string): Observable<TourLog[]> {
    return this.http.get<TourLog[]>(`${this.apiUrl}/tour/${tourId}`);
  }

  // GET: /api/logs
  // Alle Logs laden für die Home-Übersicht
  getAllLogs(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // POST: /api/logs/tour/{tourId}
  // Ein neues Log zu einer Tour hinzufügen
  addLog(tourId: string, log: TourLogBackendDto): Observable<TourLog> {
    return this.http.post<TourLog>(`${this.apiUrl}/tour/${tourId}`, log);
  }

  // Update: /api/logs/{logId}
  updateLog(logId: string, logData: TourLogBackendDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${logId}`, logData);
  }

  // DELETE: /api/logs/{logId}
  // Ein Log löschen
  deleteLog(logId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${logId}`);
  }
}
