import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { TourLog } from '../../models/tour-log.model';
import { Tour } from '../../models/tour.model';
import { Difficulty } from '../../models/enums.model';

@Component({
  selector: 'app-create-tour-log',
  standalone: true,
  imports: [Navbar, Button, FormsModule, CommonModule],
  templateUrl: './create-tour-log.html',
  styleUrl: './create-tour-log.scss',
})
export class CreateTourLog implements OnInit {
  tours: Tour[] = [];
  tourLog: TourLog = {
    logId: 'log-' + Date.now(),
    tourId: '',
    name: '',
    dateTime: '',
    comment: '',
    difficulty: '' as Difficulty, // Force selection
    totalDistance: 0,
    totalTime: 0,
    rating: 0,
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
    // Prevent future dates for logs
    const today = new Date();
    this.todayDate = today.toISOString().split('T')[0];

    // Load available tours for the dropdown
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');

    // Check if we are editing an existing log
    const editId = this.route.snapshot.paramMap.get('id');
    if (editId) {
      this.isEditMode = true;
      const logs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      const existing = logs.find((l: TourLog) => l.logId === editId);
      if (existing) {
        this.tourLog = existing;
        this.loadExistingImage();
      }
    } else {
      // If creating new, check if a tourId was passed via query params (from Tour Details page)
      this.route.queryParams.subscribe((params) => {
        if (params['tourId']) {
          this.tourLog.tourId = params['tourId'];
          this.updateLogName();
        }
      });
    }
  }

  loadExistingImage() {
    if (this.tourLog.imagePath) {
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
      const imgRecord = globalImages.find((img: any) => img.filename === this.tourLog.imagePath);
      if (imgRecord) {
        this.previewUrl = imgRecord.data;
      }
    }
  }

  // Auto-fill the log name based on the selected tour to keep data consistent
  updateLogName() {
    const selectedTour = this.tours.find((t) => t.tourId === this.tourLog.tourId);
    if (selectedTour) {
      this.tourLog.name = `${selectedTour.name} Log`;
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

  saveLog() {
    let imageFilename = this.tourLog.imagePath || '';

    // Handle Image upload
    if (this.selectedFile && this.previewUrl) {
      imageFilename = `log-${Date.now()}-${this.selectedFile.name}`;
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
      alert('An image is required for this log.');
      return;
    }

    this.tourLog.imagePath = imageFilename;

    const existingLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');

    if (this.isEditMode) {
      const index = existingLogs.findIndex((l: TourLog) => l.logId === this.tourLog.logId);
      if (index !== -1) existingLogs[index] = this.tourLog;
    } else {
      existingLogs.push(this.tourLog);
    }

    try {
      localStorage.setItem('tourLogs', JSON.stringify(existingLogs));
      // Navigate back to the tour details if applicable, else home
      if (this.tourLog.tourId) {
        this.router.navigate(['/tour-details', this.tourLog.tourId]);
      } else {
        this.router.navigate(['/home']);
      }
    } catch (e) {
      alert('Error saving log data.');
    }
  }
}
