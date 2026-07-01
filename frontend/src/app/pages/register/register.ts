import { Component, ChangeDetectorRef } from '@angular/core';
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
  errorField: string | null = null;
  formSubmitted = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  isPasswordValid(): boolean {
    const pwd = this.registerData.password;

    if (!pwd || pwd.length < 8) return false;
    if (!/[a-z]/.test(pwd)) return false;
    if (!/[A-Z]/.test(pwd)) return false;
    if (!/\d/.test(pwd)) return false;
    if (!/[!@#$%^&*]/.test(pwd)) return false;

    return true;
  }

  isFormValid(): boolean {
    return (
      this.registerData.username.length >= 3 &&
      this.registerData.email.includes('@') &&
      this.isPasswordValid()
    );
  }

  getPasswordErrorMessage(): string {
    const pwd = this.registerData.password;
    const errors: string[] = [];

    if (!pwd || pwd.length < 8) errors.push('8 Zeichen');
    if (!/[a-z]/.test(pwd)) errors.push('Kleinbuchstabe');
    if (!/[A-Z]/.test(pwd)) errors.push('Großbuchstabe');
    if (!/\d/.test(pwd)) errors.push('Ziffer');
    if (!/[!@#$%^&*]/.test(pwd)) errors.push('Sonderzeichen');

    return errors.length > 0 ? 'Fehlt: ' + errors.join(', ') : '';
  }

  onRegister() {
    this.formSubmitted = true;
    this.errorMessage = null;
    this.errorField = null;

    if (!this.registerData.username || this.registerData.username.length < 3) {
      return;
    }

    if (!this.registerData.email || !this.registerData.email.includes('@')) {
      return;
    }

    if (!this.isPasswordValid()) {
      return;
    }

    const registerPayload = {
      username: this.registerData.username,
      email: this.registerData.email,
      password: this.registerData.password,
      firstName: this.registerData.firstName || null,
      lastName: this.registerData.lastName || null,
      bio: this.registerData.bio || null
    };

    this.authService.register(registerPayload).subscribe({
      next: () => {
        this.formSubmitted = false;
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorField = err.error?.field ?? null;
        this.errorMessage = err.error?.message ?? 'Registrierung fehlgeschlagen.';
        this.cdr.detectChanges();
      }
    });
  }
}