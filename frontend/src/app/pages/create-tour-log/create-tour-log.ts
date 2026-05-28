import { Component } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Button } from '../../components/button/button';
import { TourLog } from '../../models/tour-log.model';
import { Difficulty } from '../../models/enums.model';

@Component({
  selector: 'app-create-tour-log',
  imports: [Navbar, Button, FormsModule],
  templateUrl: './create-tour-log.html',
  styleUrl: './create-tour-log.scss',
})
export class CreateTourLog {
  tourLog: TourLog = {
    logId: '',
    tourId: '',
    name: '',
    dateTime: new Date(),
    comment: '',
    difficulty: Difficulty.Easy,
    totalDistance: 0,
    totalTime: 0,
    rating: 0,
  };

  constructor(private router: Router) {}

  saveLog() {
    const existingLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
    existingLogs.push(this.tourLog);
    localStorage.setItem('tourLogs', JSON.stringify(existingLogs));
    this.router.navigate(['/home']);
  }
}
