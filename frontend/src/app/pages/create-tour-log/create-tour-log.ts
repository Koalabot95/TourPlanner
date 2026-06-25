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
import { TourService } from '../../services/tour.service'; 
import { TourLogService } from '../../services/tour-log.service'; 

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
    tourId: '',
    name: '',
    dateTime: '', 
    comment: '',
    difficulty: Difficulty.Easy, // Standardwert
    totalDistance: 0,
    totalTime: 0,
    rating: 0
  };

  previewUrl: string | null = null;
  isEditMode: boolean = false;
  todayDate: string = '';
  private selectedFile: File | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private tourService: TourService, 
    private tourLogService: TourLogService 
  ) { }

  ngOnInit() {
    this.todayDate = new Date().toISOString().split('T')[0];
    this.tourLog.dateTime = new Date().toISOString(); 

    this.tourService.getTours().subscribe({
      next: (data) => {
        this.tours = data;
        this.handleRoutingContext();
      },
      error: (err: any) => console.error('Fehler beim Laden der Touren:', err)
    });
  }

  updateLogName() {
    const selectedTour = this.tours.find((t) => t.tourId === this.tourLog.tourId);
    if (selectedTour) {
      this.tourLog.name = `${selectedTour.name} Log`;
    }
  }

  onImageSelected(event: { file: File; preview: string }) {
    this.selectedFile = event.file;
    this.previewUrl = event.preview;
    // TODO: Später per FormData an /api/images senden
  }

  saveLog() {
    if (this.isEditMode) {
      // TODO: Später updateLog implementieren
      this.navigateAfterSave();
    } else {
      // API-Call
      this.tourLogService.addLog(this.tourLog.tourId, this.tourLog).subscribe({
        next: (savedLog: any) => {
          console.log('Log erfolgreich gespeichert:', savedLog);
          this.navigateAfterSave();
        },
        error: (err: any) => {
          console.error('Fehler beim Speichern des Logs:', err);
          alert('Das Log konnte nicht gespeichert werden.');
        }
      });
    }
  }

  private handleRoutingContext() {
    const editId = this.route.snapshot.paramMap.get('id');
    if (editId) {
      this.loadExistingLogForEditing(editId);
    } else {
      this.prefillTourIdFromQueryParams();
    }
  }

  private loadExistingLogForEditing(logId: string) {
    this.isEditMode = true;
    if (this.tourLog.tourId) {
      this.tourLogService.getLogsForTour(this.tourLog.tourId).subscribe((logs: any[]) => {
        const existingLog = logs.find((l: any) => l.logId === logId);
        if (existingLog) this.tourLog = existingLog;
      });
    }
  }

  private prefillTourIdFromQueryParams() {
    this.route.queryParams.subscribe((params) => {
      if (params['tourId']) {
        this.tourLog.tourId = params['tourId'];
        this.updateLogName();
      }
    });
  }

  private navigateAfterSave() {
    if (this.tourLog.tourId) {
      this.router.navigate(['/tour-details', this.tourLog.tourId]);
    } else {
      this.router.navigate(['/home']);
    }
  }
}
