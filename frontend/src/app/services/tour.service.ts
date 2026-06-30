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

  // Spiegelt: GetAllToursAsync
  getTours(): Observable<Tour[]> {
    return this.http.get<Tour[]>(this.apiUrl);
  }

  // Spiegelt: GetTourByIdAsync
  getTourById(id: string): Observable<Tour> {
    return this.http.get<Tour>(`${this.apiUrl}/${id}`);
  }

  // Spiegelt: CreateTourAsync
  createTour(tour: Tour): Observable<Tour> {
    return this.http.post<Tour>(this.apiUrl, tour);
  }

  // Spiegelt: UpdateTourAsync
  updateTour(id: string, tour: Tour): Observable<Tour> {
    return this.http.put<Tour>(`${this.apiUrl}/${id}`, tour);
  }

  // Spiegelt: DeleteTourAsync
  deleteTour(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
