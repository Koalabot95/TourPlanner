import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Button } from '../../components/button/button';
import { FormField } from '../../components/form-field/form-field';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Button, FormField],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  registerData = {
    username: '',
    email: '',
    password: '', 
    firstName: '',
    lastName: '',
    bio: ''
  };

  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onRegister() {
    this.errorMessage = null;

    this.authService.register(this.registerData).subscribe({
      next: (response) => {
        console.log('User erfolgreich registriert!', response);
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registrierung fehlgeschlagen:', err);
        this.errorMessage = err.error?.message || 'Registrierung fehlgeschlagen. Bitte versuche es erneut.';
      }
    });
  }
}
