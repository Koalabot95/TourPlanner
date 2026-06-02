import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { FormField } from '../../components/form-field/form-field';
import { ImageUpload } from '../../components/image-upload/image-upload';
import { Tour } from '../../models/tour.model';
import { TransportType } from '../../models/enums.model';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [Navbar, Button, FormField, ImageUpload, FormsModule, CommonModule],
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
    transportType: '' as TransportType,
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
    this.todayDate = new Date().toISOString().split('T')[0];
    this.checkAndLoadEditMode();
  }

  // Updates the selected file and preview when the user uploads an image
  onImageSelected(event: { file: File; preview: string }) {
    this.selectedFile = event.file;
    this.previewUrl = event.preview;
  }

  saveTour() {
    const isImageSaved = this.processAndSaveImage();

    // Abort saving if image processing failed or is missing on creation
    if (!isImageSaved) return;

    this.saveTourDataToStorage();
    this.router.navigate(['/home']);
  }

  // Checks the URL for an ID and loads existing data if found
  private checkAndLoadEditMode() {
    const editId = this.route.snapshot.paramMap.get('id');
    if (!editId) return; // Exit early if creating a new tour

    this.isEditMode = true;
    const tours = JSON.parse(localStorage.getItem('tours') || '[]');
    const existingTour = tours.find((t: Tour) => t.tourId === editId);

    if (existingTour) {
      this.tour = existingTour;
      this.loadExistingImagePreview(this.tour.imagePath ?? '');
    }
  }

  // Fetches the image data from local storage if an image path exists
  private loadExistingImagePreview(imagePath: string) {
    if (!imagePath) return;

    const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
    const imgRecord = globalImages.find((img: any) => img.filename === imagePath);

    if (imgRecord) {
      this.previewUrl = imgRecord.data;
    }
  }

  // Handles naming, saving, and storing the image file. Returns true if successful/valid.
  private processAndSaveImage(): boolean {
    let imageFilename = this.tour.imagePath || '';

    // If the user selected a new file, process it
    if (this.selectedFile && this.previewUrl) {
      imageFilename = `tour-${Date.now()}-${this.selectedFile.name}`;
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
      globalImages.push({ filename: imageFilename, data: this.previewUrl });

      try {
        localStorage.setItem('global_images', JSON.stringify(globalImages));
      } catch (e) {
        alert('Storage full! Could not save image.');
        return false; // Stop the save process
      }
    }

    // Require an image if we are creating a brand new tour
    if (!imageFilename && !this.isEditMode) {
      return false;
    }

    this.tour.imagePath = imageFilename;
    return true; // Image processed successfully (or wasn't required to change)
  }

  // Pushes the finalized tour object into the local storage array
  private saveTourDataToStorage() {
    const existingTours = JSON.parse(localStorage.getItem('tours') || '[]');

    if (this.isEditMode) {
      const index = existingTours.findIndex((t: Tour) => t.tourId === this.tour.tourId);
      if (index !== -1) existingTours[index] = this.tour;
    } else {
      existingTours.push(this.tour);
    }

    localStorage.setItem('tours', JSON.stringify(existingTours));
  }
}
