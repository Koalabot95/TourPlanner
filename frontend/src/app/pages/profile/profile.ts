import { Component, OnInit } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile',
  imports: [Navbar, RouterLink, Button],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile implements OnInit {
  user = {
    firstName: 'John',
    lastName: 'Doe',
    email: 'adventurer@email.com',
    joinDate: '2026-05-01',
    bio: 'Passionate hiker and cyclist exploring the world one trail at a time.',
  };

  constructor(private router: Router) {}

  ngOnInit() {
    // In a real app, you would fetch the user data from a Service here
  }

  signOut() {
    // Clear tokens/session logic here
    this.router.navigate(['/login']);
  }

  deleteAccount() {
    if (confirm('Are you sure you want to delete your account?')) {
      // Logic to delete account
    }
  }
}
