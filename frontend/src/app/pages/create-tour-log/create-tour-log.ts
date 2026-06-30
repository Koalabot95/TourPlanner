import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { FormField } from '../../components/form-field/form-field';
import { ImageUpload } from '../../components/image-upload/image-upload';
import { TourLog, TourLogBackendDto } from '../../models/tour-log.model';
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
    this.updateLogName();

    if (!this.tourLog.tourId) {
      alert('Please select a tour first.');
      return;
    }

    let difficultyString = 'Easy'; 

      if (this.tourLog.difficulty === Difficulty.Medium || (this.tourLog.difficulty as any) === 'Medium' || (this.tourLog.difficulty as any) === 1) {
        difficultyString = 'Medium';
      } else if (this.tourLog.difficulty === Difficulty.Hard || (this.tourLog.difficulty as any) === 'Hard' || (this.tourLog.difficulty as any) === 2) {
        difficultyString = 'Hard';
      }

    // Bereinige das Objekt, um sicherzustellen, dass die numerischen Felder korrekt sind
    const logToSend: TourLogBackendDto = {
      tourId: this.tourLog.tourId,
      name: this.tourLog.name,
      // .toString() garantiert TypeScript, dass hier ein reiner string übergeben wird
      dateTime: this.tourLog.dateTime ? this.tourLog.dateTime.toString() : '',
      comment: this.tourLog.comment,
      totalDistance: +this.tourLog.totalDistance,
      totalTime: +this.tourLog.totalTime,
      rating: +this.tourLog.rating,
      difficulty: difficultyString
    };

    // Holt die Log-ID aus den Routenparametern, falls wir uns im Edit-Modus befinden
    const logId = this.route.snapshot.paramMap.get('id');

    if (this.isEditMode && logId) {
      // 1. UPDATE MODUS: Bestehendes Log ändern
      this.tourLogService.updateLog(logId, logToSend).subscribe({
        next: (updatedLog: any) => {
          console.log('Log erfolgreich aktualisiert:', updatedLog);
          this.navigateAfterSave();
        },
        error: (err: any) => {
          console.error('Fehler beim Aktualisieren des Logs:', err);
          alert('Das Log konnte nicht aktualisiert werden.');
        }
      });

    } else {
      // 2. CREATE MODUS: Neues Log anlegen 
      this.tourLogService.addLog(this.tourLog.tourId, logToSend).subscribe({
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
    // Holt die ID aus der URL-Route (/edit-tour-log/:id)
    const editId = this.route.snapshot.paramMap.get('id');

    if (editId) {
      console.log('System-Check: Edit-ID in URL gefunden:', editId);
      this.loadExistingLogForEditing(editId);
    } else {
      this.isEditMode = false;
    }
  }

  private loadExistingLogForEditing(logId: string) {
    this.isEditMode = true;

    this.route.queryParams.subscribe(params => {
      const tourIdFromUrl = params['tourId'];

      if (tourIdFromUrl) {
        // 2. Alle Logs für diese Tour laden, um das spezifische Log zu finden
        this.tourLogService.getLogsForTour(tourIdFromUrl).subscribe({
          next: (logs: any[]) => {
            // 3. Passendes Log anhand der logId finden (Case-Insensitive Fallback eingebaut)
            const existingLog = logs.find((l: any) =>
              l.logId === logId || l.tourLogId === logId || l.id === logId
            );

            if (existingLog) {
              // Daten sicher in unser bestehendes Modell mergen
              this.tourLog = {
                ...this.tourLog,
                ...existingLog,
                tourId: tourIdFromUrl // Garantiert die korrekte Bindung ans Dropdown
              };

              // Datum für das HTML-Feld <input type="date"> auf YYYY-MM-DD trimmen
              if (this.tourLog.dateTime) {
                this.tourLog.dateTime = this.tourLog.dateTime.toString().split('T')[0];
              }

              this.updateLogName();
              console.log('Log erfolgreich für Edit geladen:', this.tourLog);
            } else {
              console.warn(`Log mit ID ${logId} wurde unter dieser Tour nicht gefunden.`);
            }
          },
          error: (err) => console.error('Fehler beim Laden der Logs vom Backend:', err)
        });
      } else {
        console.error('Edit-Modus gestartet, aber keine tourId in den QueryParams gefunden!');
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
