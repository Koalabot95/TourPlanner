import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { FormField } from '../../components/form-field/form-field';
import { ImageUpload } from '../../components/image-upload/image-upload';
import { TourLog } from '../../models/tour-log.model';
import { Tour } from '../../models/tour.model';
import { Difficulty } from '../../models/enums.model';
import { ImageService } from '../../services/image.service';

@Component({
  selector: 'app-create-tour-log',
  standalone: true,
  imports: [Navbar, Button, FormField, ImageUpload, FormsModule, CommonModule],
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
    difficulty: '' as Difficulty,
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
    private imageService: ImageService,
  ) {}

  ngOnInit() {
    this.todayDate = new Date().toISOString().split('T')[0];
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');

    this.handleRoutingContext();
  }

  // Changes the log name when a different tour is selected in the dropdown
  updateLogName() {
    const selectedTour = this.tours.find((t) => t.tourId === this.tourLog.tourId);
    if (selectedTour) {
      this.tourLog.name = `${selectedTour.name} Log`;
    }
  }

  // File upload trigger
  onImageSelected(event: { file: File; preview: string }) {
    this.selectedFile = event.file;
    this.previewUrl = event.preview;
  }

  saveLog() {
    const isImageSaved = this.processAndSaveImage();
    if (!isImageSaved) return;
    this.saveLogDataToStorage();
    this.navigateAfterSave();
  }

  // Determines if we are editing an existing log OR pre-filling based on query params
  private handleRoutingContext() {
    const editId = this.route.snapshot.paramMap.get('id');
    if (editId) {
      this.loadExistingLogForEditing(editId);
    } else {
      this.prefillTourIdFromQueryParams();
    }
  }

  // Loads log data and image when editing an existing entry
  private loadExistingLogForEditing(logId: string) {
    this.isEditMode = true;
    const logs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
    const existingLog = logs.find((l: TourLog) => l.logId === logId);
    if (existingLog) {
      this.tourLog = existingLog;
      this.loadExistingImagePreview(this.tourLog.imagePath ?? '');
    }
  }

  // Prefills the dropdown if the user navigated here from a specific tour details page
  private prefillTourIdFromQueryParams() {
    this.route.queryParams.subscribe((params) => {
      if (params['tourId']) {
        this.tourLog.tourId = params['tourId'];
        this.updateLogName();
      }
    });
  }

  // Fetches the image data via the ImageService
  private loadExistingImagePreview(imagePath: string) {
    this.previewUrl = this.imageService.getPreviewUrl(imagePath);
  }

  // Delegates image naming and storage to the ImageService
  private processAndSaveImage(): boolean {
    const newFilename = this.imageService.processAndSaveImage(
      this.selectedFile,
      this.previewUrl,
      this.tourLog.imagePath,
      'log',
    );
    if (!newFilename && !this.isEditMode) return false;
    this.tourLog.imagePath = newFilename || '';
    return true;
  }

  // Pushes the log object into the localStorage array
  private saveLogDataToStorage() {
    const existingLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
    if (this.isEditMode) {
      const index = existingLogs.findIndex((l: TourLog) => l.logId === this.tourLog.logId);
      if (index !== -1) existingLogs[index] = this.tourLog;
    } else {
      existingLogs.push(this.tourLog);
    }
    localStorage.setItem('tourLogs', JSON.stringify(existingLogs));
  }

  // Determines where to send the user after successfully saving
  private navigateAfterSave() {
    if (this.tourLog.tourId) {
      this.router.navigate(['/tour-details', this.tourLog.tourId]);
    } else {
      this.router.navigate(['/home']);
    }
  }
}
