import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { TourCard } from '../../components/tour-card/tour-card';
import { TourLogCard } from '../../components/tour-log-card/tour-log-card';
import { Tour } from '../../models/tour.model';
import { TourLog } from '../../models/tour-log.model';
import { TourService } from '../../services/tour.service'; 
import { TourLogService } from '../../services/tour-log.service'; 

@Component({
  selector: 'app-tour-details',
  standalone: true,
  imports: [CommonModule, Navbar, Button, TourCard, RouterLink, TourLogCard],
  templateUrl: './tour-details.html',
  styleUrl: './tour-details.scss',
})
export class TourDetails implements OnInit {
  tour: Tour | null = null;
  tourLogs: TourLog[] = [];
  imageUrl: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tourService: TourService, 
    private tourLogService: TourLogService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    // 1. Spezifische Tour direkt über die ID laden
    this.tourService.getTourById(id).subscribe({
      next: (tourData) => {
        this.tour = tourData;
        this.imageUrl = this.tour?.mapSnapshotPath || '';
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        console.error('Fehler beim Laden der Tour-Details:', err);
      }
    });

    this.tourLogService.getLogsForTour(id).subscribe({
      next: (logs) => {
        this.tourLogs = logs;
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Fehler beim Laden der Tour-Logs:', err)
    });
  }

  deleteTour() {
    const id = this.tour?.tourId;
    if (!id) return;

    if (confirm('Are you sure you want to delete this tour? This will also delete all associated logs.')) {
      this.tourService.deleteTour(id).subscribe({
        next: () => {
          console.log('Tour erfolgreich gelöscht!');
          this.router.navigate(['/home']); // Zurück zur Übersicht springen
        },
        error: (err) => console.error('Fehler beim Löschen der Tour:', err)
      });
    }
  }

  deleteLog(logId: string) {
    if (confirm('Are you sure you want to delete this log?')) {
      this.tourLogService.deleteLog(logId).subscribe({
        next: () => {
          this.tourLogs = this.tourLogs.filter((l: TourLog) => l.logId !== logId);
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Fehler beim Löschen des Logs:', err)
      });
    }
  }
}
