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
  ) {}

  ngOnInit() {
    this.todayDate = new Date().toISOString().split('T')[0];
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');

    const editId = this.route.snapshot.paramMap.get('id');
    if (editId) {
      this.isEditMode = true;
      const logs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
      const existing = logs.find((l: TourLog) => l.logId === editId);
      if (existing) {
        this.tourLog = existing;
        if (this.tourLog.imagePath) {
          const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
          const imgRecord = globalImages.find(
            (img: any) => img.filename === this.tourLog.imagePath,
          );
          if (imgRecord) this.previewUrl = imgRecord.data;
        }
      }
    } else {
      this.route.queryParams.subscribe((params) => {
        if (params['tourId']) {
          this.tourLog.tourId = params['tourId'];
          this.updateLogName();
        }
      });
    }
  }

  updateLogName() {
    const selectedTour = this.tours.find((t) => t.tourId === this.tourLog.tourId);
    if (selectedTour) this.tourLog.name = `${selectedTour.name} Log`;
  }

  onImageSelected(event: { file: File; preview: string }) {
    this.selectedFile = event.file;
    this.previewUrl = event.preview;
  }

  saveLog() {
    let imageFilename = this.tourLog.imagePath || '';

    if (this.selectedFile && this.previewUrl) {
      imageFilename = `log-${Date.now()}-${this.selectedFile.name}`;
      const globalImages = JSON.parse(localStorage.getItem('global_images') || '[]');
      globalImages.push({ filename: imageFilename, data: this.previewUrl });
      try {
        localStorage.setItem('global_images', JSON.stringify(globalImages));
      } catch (e) {
        alert('Storage full!');
        return;
      }
    }

    if (!imageFilename && !this.isEditMode) return;

    this.tourLog.imagePath = imageFilename;
    const existingLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');

    if (this.isEditMode) {
      const index = existingLogs.findIndex((l: TourLog) => l.logId === this.tourLog.logId);
      if (index !== -1) existingLogs[index] = this.tourLog;
    } else {
      existingLogs.push(this.tourLog);
    }

    localStorage.setItem('tourLogs', JSON.stringify(existingLogs));
    if (this.tourLog.tourId) {
      this.router.navigate(['/tour-details', this.tourLog.tourId]);
    } else {
      this.router.navigate(['/home']);
    }
  }
}
