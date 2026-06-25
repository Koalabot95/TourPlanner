import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { TourCard } from '../../components/tour-card/tour-card';
import { TourLogCard } from '../../components/tour-log-card/tour-log-card';
import { Tour } from '../../models/tour.model';
import { TourLog } from '../../models/tour-log.model';
import { TourService } from '../../services/tour.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, Navbar, RouterLink, Button, TourCard, TourLogCard],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  tours: Tour[] = [];
  tourLogs: TourLog[] = [];

  constructor(private tourService: TourService, private cdr: ChangeDetectorRef) { } 

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.tourService.getTours().subscribe({
      next: (tours) => {
        this.tours = tours;

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Fehler beim Laden der Tours:', err);
      }
    });

    // TourLogs noch aus localStorage bis Backend fertig ist
    this.tourLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
  }

  deleteLog(logId: string) {
    if (confirm('Are you sure you want to delete this log?')) {
      let allLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      allLogs = allLogs.filter((l: TourLog) => l.logId !== logId);
      localStorage.setItem('tourLogs', JSON.stringify(allLogs));
      this.tourLogs = allLogs;
      this.cdr.detectChanges();
    }
  }

  getImageUrl(filename: string | undefined): string | null {
    return null; // TODO: später vom Backend laden
  }
}
