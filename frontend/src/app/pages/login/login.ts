import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common'; 
import { Button } from '../../components/button/button';
import { AuthService } from '../../services/auth.service'; 

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, FormsModule, Button, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  credentials = {
    username: '',
    password: ''
  };

  errorMessage: string = ''; // <- Variable, um Fehler aus dem Backend zu speichern

  // Den AuthService im Konstruktor injizieren
  constructor(
    private router: Router,
    private authService: AuthService 
  ) {}

  redirect() {
    this.router.navigate(['/home']); 
  }

  login() {
    this.errorMessage = ''; // Fehler bei jedem Klick zurücksetzen
    console.log('Attempting login with:', this.credentials);
    
    // Service aufrufen
    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        console.log('Login erfolgreich! Token erhalten:', response);
        
        
        if (response && response.token) {
          localStorage.setItem('auth_token', response.token);
        }
        
        
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error('Login fehlgeschlagen:', err);
        this.errorMessage = err.error?.message || 'Login fehlgeschlagen. Bitte überprüfe deine Daten.';
      }
    });
  }
}
