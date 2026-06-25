import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TourLog } from '../models/tour-log.model';

@Injectable({
  providedIn: 'root'
})
export class TourLogService {
  private apiUrl = '/api/tourlogs'; 

  constructor(private http: HttpClient) { }

  // Alle Logs für eine ganz bestimmte Tour laden
  getLogsForTour(tourId: string): Observable<TourLog[]> {
    return this.http.get<TourLog[]>(`${this.apiUrl}/tour/${tourId}`);
  }

  //Alle Logs laden 
  getAllLogs(): Observable<any[]> {
    return this.http.get<any[]>('/api/tourlog/all');
  }

  // Ein neues Log zu einer Tour hinzufügen
  addLog(log: TourLog): Observable<TourLog> {
    return this.http.post<TourLog>(this.apiUrl, log);
  }

  // Ein Log löschen
  deleteLog(logId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${logId}`);
  }
}
