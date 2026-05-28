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
  // Map to cache images: filename -> base64Data
  imageCache: Map<string, string> = new Map();

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    // Load Tours
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');

    // Load Logs
    this.tourLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');

    // Load Global Images into a Map for fast lookup
    const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
    globalImages.forEach((img: { filename: string; data: string }) => {
      this.imageCache.set(img.filename, img.data);
    });
  }

  // Helper to get image URL by filename
  getImageUrl(filename: string | undefined): string | null {
    if (!filename) return null;
    return this.imageCache.get(filename) || null;
  }
}
