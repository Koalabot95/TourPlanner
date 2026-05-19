import { Component } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Button } from '../../components/button/button';

@Component({
  selector: 'app-create-tour-log',
  imports: [Navbar, Button, FormsModule],
  templateUrl: './create-tour-log.html',
  styleUrl: './create-tour-log.scss',
})
export class CreateTourLog {
  tourLog = {
    tourName: 'Mountain Trail', // Default selection
    date: '',
    distance: null,
    time: null,
    rating: '3', // Default to Good
    notes: '',
  };

  constructor(private router: Router) {}

  saveLog() {
    const existingLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
    existingLogs.push(this.tourLog);
    localStorage.setItem('tourLogs', JSON.stringify(existingLogs));

    this.router.navigate(['/home']);
  }
}
