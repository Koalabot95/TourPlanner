import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Tour } from '../../models/tour.model';
import { TransportType } from '../../models/enums.model';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [Navbar, Button, FormsModule, CommonModule],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.scss',
})
export class CreateTour implements OnInit {
  tour: Tour = {
    userId: 'default-user-id',
    tourId: 'tour-' + Date.now(),
    name: '',
    description: '',
    startLocation: '',
    endLocation: '',
    startDate: '',
    endDate: '',
    transportType: '' as TransportType, // <-- Set to empty to force user selection
    distance: 0,
    estimatedTime: 0,
    routeInformation: '',
    mapSnapshotPath: '',
    popularity: 0,
    childfriendliness: 0,
    imagePath: '',
  };

  previewUrl: string | null = null;
  isEditMode: boolean = false;
  todayDate: string = '';
  private selectedFile: File | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    // Generate today's date in YYYY-MM-DD format to prevent past dates
    const today = new Date();
    this.todayDate = today.toISOString().split('T')[0];

    const editId = this.route.snapshot.paramMap.get('id');
    if (editId) {
      this.isEditMode = true;
      const tours = JSON.parse(localStorage.getItem('tours') || '[]');
      const existing = tours.find((t: Tour) => t.tourId === editId);
      if (existing) {
        this.tour = existing;

        if (this.tour.imagePath) {
          const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
          const imgRecord = globalImages.find((img: any) => img.filename === this.tour.imagePath);
          if (imgRecord) {
            this.previewUrl = imgRecord.data;
          }
        }
      }
    }
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
    if (this.selectedFile) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewUrl = e.target.result;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  saveTour() {
    let imageFilename = this.tour.imagePath || '';

    if (this.selectedFile && this.previewUrl) {
      imageFilename = `tour-${Date.now()}-${this.selectedFile.name}`;
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
      globalImages.push({
        filename: imageFilename,
        data: this.previewUrl,
      });

      try {
        localStorage.setItem('global_images', JSON.stringify(globalImages));
      } catch (e) {
        alert('Storage full! Could not save image.');
        return;
      }
    }

    if (!imageFilename && !this.isEditMode) {
      alert('An image is required to create a tour.');
      return;
    }

    this.tour.imagePath = imageFilename;

    const existingTours = JSON.parse(localStorage.getItem('tours') || '[]');

    if (this.isEditMode) {
      const index = existingTours.findIndex((t: Tour) => t.tourId === this.tour.tourId);
      if (index !== -1) existingTours[index] = this.tour;
    } else {
      existingTours.push(this.tour);
    }

    try {
      localStorage.setItem('tours', JSON.stringify(existingTours));
      this.router.navigate(['/home']);
    } catch (e) {
      alert('Error saving tour data.');
    }
  }
}
