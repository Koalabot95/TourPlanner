import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth';

  constructor(private http: HttpClient) { }

  register(registerData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, registerData);
  }

  // Nutzt 'tap', um den Token direkt nach erfolgreichem Login abzuspeichern
  login(loginData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginData).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem('auth_token', response.token);
        }
      })
    );
  }

  // Hilfsmethode, um den Token für den Interceptor auszulesen
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  // Zum Ausloggen einfach den Token kicken
  logout() {
    localStorage.removeItem('auth_token');
  }
}
