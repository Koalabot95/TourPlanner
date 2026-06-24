import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { FormField } from '../../components/form-field/form-field';
import { ImageUpload } from '../../components/image-upload/image-upload';
import { Tour } from '../../models/tour.model';
import { TransportMode } from '../../models/enums.model'; 
import { TourService } from '../../services/tour.service'; 

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [Navbar, Button, FormField, ImageUpload, FormsModule, CommonModule],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.scss',
})
export class CreateTour implements OnInit {
  tour: Tour = {
    userId: '00000000-0000-0000-0000-000000000000', 
    name: '',
    description: '',
    startLocation: '',
    endLocation: '',
    startDate: '', 
    endDate: '',   
    transportType: TransportMode.Walking,
    distance: 0,
    estimatedTime: 0,
    mapSnapshotPath: '',
    popularity: 0,
    childFriendliness: 0
  };

  previewUrl: string | null = null;
  isEditMode: boolean = false;
  todayDate: string = '';
  private selectedFile: File | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private tourService: TourService 
  ) { }

  ngOnInit() {
    this.todayDate = new Date().toISOString().split('T')[0];
    this.tour.startDate = this.todayDate;
    this.tour.endDate = this.todayDate;
    this.checkAndLoadEditMode();
  }

  onImageSelected(event: { file: File; preview: string }) {
    this.selectedFile = event.file;
    this.previewUrl = event.preview;
    // TODO: Später per FormData an die API /api/images senden
  }

  saveTour() {
    if (this.isEditMode) {
      // TODO: Später updateTour im Service aufrufen, falls Edit-Modus aktiv
      this.router.navigate(['/home']);
    } else {
      this.tourService.createTour(this.tour).subscribe({
        next: (savedTour) => {
          console.log('Tour erfolgreich in Postgres gespeichert:', savedTour);
          this.router.navigate(['/home']);
        },
        error: (err) => {
          console.error('Fehler beim Speichern der Tour:', err);
          alert('Die Tour konnte nicht in der Datenbank gespeichert werden.');
        }
      });
    }
  }

  private checkAndLoadEditMode() {
    const editId = this.route.snapshot.paramMap.get('id');
    if (!editId) return;

    this.isEditMode = true;

    this.tourService.getTours().subscribe(tours => {
      const existingTour = tours.find((t: Tour) => t.tourId === editId);
      if (existingTour) {
        this.tour = existingTour;
      }
    });
  }
}
