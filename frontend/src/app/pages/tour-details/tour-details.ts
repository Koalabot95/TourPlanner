import { Component, OnInit } from '@angular/core';
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
    private tourLogService: TourLogService 
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    // 1. Tour aus der API laden
    this.tourService.getTours().subscribe({
      next: (tours) => {
        this.tour = tours.find((t: Tour) => t.tourId === id) || null;
        this.imageUrl = this.tour?.mapSnapshotPath || '';
      },
      error: (err) => console.error('Fehler beim Laden der Tour-Details:', err)
    });

    // 2. Zugehörige Logs aus der API laden
    this.tourLogService.getLogsForTour(id).subscribe({
      next: (logs) => {
        this.tourLogs = logs;
      },
      error: (err) => console.error('Fehler beim Laden der Tour-Logs:', err)
    });
  }

  deleteTour() {
    if (confirm('Are you sure you want to delete this tour? This will also delete all associated logs.')) {
      // TODO: Später deleteTour im TourService implementieren
      // Da OnDelete(DeleteBehavior.Cascade) im Backend aktiv ist, löscht Postgres die Logs automatisch!
      this.router.navigate(['/home']);
    }
  }

  deleteLog(logId: string) {
    if (confirm('Are you sure you want to delete this log?')) {
      this.tourLogService.deleteLog(logId).subscribe({
        next: () => {
          // Liste nach dem Löschen aktualisieren
          this.tourLogs = this.tourLogs.filter((l: TourLog) => l.logId !== logId);
        },
        error: (err) => console.error('Fehler beim Löschen des Logs:', err)
      });
    }
  }
}
