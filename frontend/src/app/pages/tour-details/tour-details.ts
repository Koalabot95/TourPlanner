import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Card } from '../../components/card/card';
import { Tour } from '../../models/tour.model';

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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    const tours = JSON.parse(localStorage.getItem('tours') || '[]');
    this.tour = tours.find((t: Tour) => t.tourId === id) || null;

    if (this.tour && this.tour.imagePath) {
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
      const imgRecord = globalImages.find((img: any) => img.filename === this.tour?.imagePath);
      if (imgRecord) {
        this.imageUrl = imgRecord.data;
      }
    }
  }

  deleteTour() {
    if (confirm('Are you sure you want to delete this tour? This action cannot be undone.')) {
      let tours = JSON.parse(localStorage.getItem('tours') || '[]');
      tours = tours.filter((t: Tour) => t.tourId !== this.tour?.tourId);
      localStorage.setItem('tours', JSON.stringify(tours));
      this.router.navigate(['/home']);
    }
  }
}
