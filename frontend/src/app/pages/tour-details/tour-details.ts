import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Card } from '../../components/card/card';
import { Tour } from '../../models/tour.model';
import { TourLog } from '../../models/tour-log.model';

@Component({
  selector: 'app-tour-details',
  standalone: true,
  imports: [CommonModule, Navbar, Button, Card, RouterLink],
  templateUrl: './tour-details.html',
  styleUrl: './tour-details.scss',
})
export class TourDetails implements OnInit {
  tour: Tour | null = null;
  imageUrl: string | null = null;

  tourLogs: TourLog[] = [];
  imageCache: Map<string, string> = new Map();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const id = this.route.snapshot.paramMap.get('id');

    // Load Tour
    const tours = JSON.parse(localStorage.getItem('tours') || '[]');
    this.tour = tours.find((t: Tour) => t.tourId === id) || null;

    // Load Image Cache
    const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
    globalImages.forEach((img: { filename: string; data: string }) => {
      this.imageCache.set(img.filename, img.data);
    });

    if (this.tour && this.tour.imagePath) {
      this.imageUrl = this.imageCache.get(this.tour.imagePath) || null;
    }

    // Load Associated Logs
    const allLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
    this.tourLogs = allLogs.filter((l: TourLog) => l.tourId === id);
  }

  getImageUrl(filename: string | undefined): string | null {
    if (!filename) return null;
    return this.imageCache.get(filename) || null;
  }

  deleteTour() {
    if (
      confirm(
        'Are you sure you want to delete this tour? This will also delete all associated logs.',
      )
    ) {
      // Delete Tour
      let tours = JSON.parse(localStorage.getItem('tours') || '[]');
      tours = tours.filter((t: Tour) => t.tourId !== this.tour?.tourId);
      localStorage.setItem('tours', JSON.stringify(tours));

      // Cascade Delete Logs
      let allLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      allLogs = allLogs.filter((l: TourLog) => l.tourId !== this.tour?.tourId);
      localStorage.setItem('tourLogs', JSON.stringify(allLogs));

      this.router.navigate(['/home']);
    }
  }

  deleteLog(logId: string) {
    if (confirm('Are you sure you want to delete this log?')) {
      let allLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      allLogs = allLogs.filter((l: TourLog) => l.logId !== logId);
      localStorage.setItem('tourLogs', JSON.stringify(allLogs));

      // Refresh log list
      this.tourLogs = allLogs.filter((l: TourLog) => l.tourId === this.tour?.tourId);
    }
  }
}
