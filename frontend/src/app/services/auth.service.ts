import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root' // Service überall in der App verfügbar
})
export class AuthService {
  private apiUrl = '/api/auth'; 

  constructor(private http: HttpClient) { }

  // Methode für die Registrierung (schickt RegisterDto ans Backend)
  register(registerData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, registerData);
  }

  // Methode für den Login (schickt LoginData ans Backend)
  login(loginData: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginData);
  }
}
