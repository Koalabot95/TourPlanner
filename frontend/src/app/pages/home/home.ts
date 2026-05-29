import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Tour } from '../../models/tour.model';
import { TourLog } from '../../models/tour-log.model';
import { Card } from '../../components/card/card';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, Navbar, RouterLink, Button, Card],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  tours: Tour[] = [];
  tourLogs: TourLog[] = [];
  imageCache: Map<string, string> = new Map();

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');
    this.tourLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');

    const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
    globalImages.forEach((img: { filename: string; data: string }) => {
      this.imageCache.set(img.filename, img.data);
    });
  }

  getImageUrl(filename: string | undefined): string | null {
    if (!filename) return null;
    return this.imageCache.get(filename) || null;
  }

  // --- NEW: Delete Log from Home Page ---
  deleteLog(logId: string) {
    if (confirm('Are you sure you want to delete this log?')) {
      let allLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      allLogs = allLogs.filter((l: TourLog) => l.logId !== logId);
      localStorage.setItem('tourLogs', JSON.stringify(allLogs));

      // Update the UI immediately
      this.tourLogs = allLogs;
    }
  }
}
