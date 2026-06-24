import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tour } from '../models/tour.model';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  
  private apiUrl = '/api/tours';

  constructor(private http: HttpClient) { }

  // Alle Touren vom Backend laden
  getTours(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // Eine neue Tour an das Backend senden und in der DB speichern
  createTour(tour: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, tour);
  }
}
