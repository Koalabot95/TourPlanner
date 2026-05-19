import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  credentials = {
    username: '',
    password: ''
  };

  constructor(private router: Router) {}

  login() {
    console.log('Attempting login with:', this.credentials);
    // Delegate authentication to an AuthService here...
    
    // On success:
    this.router.navigate(['/home']);
  }
}