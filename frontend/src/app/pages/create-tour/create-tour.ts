import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
export class CreateTour {
  tour: Tour = {
    userId: 'default-user-id',
    tourId: 'tour-' + Date.now(),
    name: '',
    description: '',
    startLocation: '',
    endLocation: '',
    startDate: '',
    endDate: '',
    transportType: TransportType.Hike,
    distance: 0,
    estimatedTime: 0,
    routeInformation: '',
    mapSnapshotPath: '',
    popularity: 0,
    childfriendliness: 0,
    imagePath: '', // Will store only the filename
  };

  previewUrl: string | null = null;
  private selectedFile: File | null = null;

  constructor(private router: Router) {}

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
    // 1. Handle Image Storage (Global)
    let imageFilename = '';

    if (this.selectedFile && this.previewUrl) {
      // Create a unique filename
      imageFilename = `tour-${Date.now()}-${this.selectedFile.name}`;

      // Get existing global images or initialize empty array
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');

      // Push new image entry: { filename: '...', data: 'base64...' }
      globalImages.push({
        filename: imageFilename,
        data: this.previewUrl,
      });

      // Save back to localStorage
      try {
        localStorage.setItem('global_images', JSON.stringify(globalImages));
      } catch (e) {
        alert('Storage full! Could not save image.');
        return;
      }
    }

    // 2. Assign only the filename to the tour
    this.tour.imagePath = imageFilename;

    // 3. Save Tour to LocalStorage
    const existingTours = JSON.parse(localStorage.getItem('tours') || '[]');
    existingTours.push(this.tour);

    try {
      localStorage.setItem('tours', JSON.stringify(existingTours));
      this.router.navigate(['/home']);
    } catch (e) {
      alert('Error saving tour data.');
    }
  }
}
