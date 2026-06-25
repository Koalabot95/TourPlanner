import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
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

  errorMessage: string = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  login() {
    this.errorMessage = '';
    console.log('Attempting login with:', this.credentials);

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        console.log('Login erfolgreich! Token erhalten:', response);

        // Das Speichern übernimmt der AuthService mit tap() selbst
        if (response && response.token) {
          this.router.navigate(['/home']);
        } else {
          this.errorMessage = 'Login fehlgeschlagen: Kein Token erhalten.';
        }
      },
      error: (err) => {
        console.error('Login fehlgeschlagen:', err);
        this.errorMessage = err.error?.message || 'Login fehlgeschlagen. Bitte überprüfe deine Daten.';
        this.cdr.detectChanges();
      }
    });
  }
}
