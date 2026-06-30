import { Component, OnInit, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import * as L from 'leaflet';
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
export class TourDetails implements OnInit, AfterViewInit {
  tour: Tour | null = null;
  tourLogs: TourLog[] = [];
  imageUrl: string = '';

  private map: L.Map | null = null;
  private mapInitialized = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tourService: TourService,
    private tourLogService: TourLogService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadData();
  }

  ngAfterViewInit() {
    // Falls die Tour-Daten bereits vor der View geladen wurden
    this.initMap();
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
        console.log('SYSTEM-CHECK TOUR-DETAILS:', this.tour);
        this.initMap();
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

  private initMap() {
    if (!this.tour || this.mapInitialized) return;

    const mapContainer = document.getElementById('tour-map');
    if (!mapContainer) return; // View noch nicht bereit

    this.mapInitialized = true;

    this.map = L.map('tour-map');

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19,
    }).addTo(this.map);

    if (this.tour.routeInformation) {
      try {
        const geo = JSON.parse(this.tour.routeInformation);
        // ORS liefert [lng, lat], Leaflet erwartet [lat, lng]
        const latLngs: L.LatLngExpression[] = geo.coordinates.map(
          (c: number[]) => [c[1], c[0]]
        );

        const routeLine = L.polyline(latLngs, { color: '#1e1e2f', weight: 4 }).addTo(this.map);

        L.marker(latLngs[0]).addTo(this.map).bindPopup('Start: ' + this.tour.startLocation);
        L.marker(latLngs[latLngs.length - 1]).addTo(this.map).bindPopup('Ziel: ' + this.tour.endLocation);

        this.map.fitBounds(routeLine.getBounds(), { padding: [30, 30] });
      } catch (e) {
        console.error('Fehler beim Parsen der Routen-Geometrie:', e);
        this.map.setView([47.5, 14.5], 7); // Fallback: Österreich-Übersicht
      }
    } else {
      console.warn('Keine routeInformation für diese Tour vorhanden.');
      this.map.setView([47.5, 14.5], 7);
    }
  }

  deleteTour() {
    const id = this.tour?.tourId;
    if (!id) return;

    if (confirm('Are you sure you want to delete this tour? This will also delete all associated logs.')) {
      this.tourService.deleteTour(id).subscribe({
        next: () => {
          console.log('Tour erfolgreich gelöscht!');
          this.router.navigate(['/home']);
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
