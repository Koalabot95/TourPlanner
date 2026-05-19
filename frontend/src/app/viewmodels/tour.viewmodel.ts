import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Tour } from '../models/tour.model';

@Injectable({
  providedIn: 'root',
})
export class TourViewModel {
  // State
  private toursSubject = new BehaviorSubject<Tour[]>([]);
  private selectedTourSubject = new BehaviorSubject<Tour | null>(null);

  // Observables for the UI to bind to using the async pipe
  tours$: Observable<Tour[]> = this.toursSubject.asObservable();
  selectedTour$: Observable<Tour | null> = this.selectedTourSubject.asObservable();

  constructor() {
    // In the final version, you will fetch this from a TourApiService.
    // For the intermediate hand-in, you can initialize with mock data here.
  }

  // Actions triggered by the View
  selectTour(tour: Tour | null): void {
    this.selectedTourSubject.next(tour);
  }

  addTour(newTour: Tour): void {
    const currentTours = this.toursSubject.getValue();
    this.toursSubject.next([...currentTours, newTour]);
  }

  updateTour(updatedTour: Tour): void {
    const currentTours = this.toursSubject.getValue();
    const index = currentTours.findIndex((t) => t.tourId === updatedTour.tourId);
    if (index !== -1) {
      currentTours[index] = updatedTour;
      this.toursSubject.next([...currentTours]);

      // Update selected tour if it's the one being modified
      if (this.selectedTourSubject.getValue()?.tourId === updatedTour.tourId) {
        this.selectTour(updatedTour);
      }
    }
  }

  deleteTour(tourId: string): void {
    const currentTours = this.toursSubject.getValue().filter((t) => t.tourId !== tourId);
    this.toursSubject.next(currentTours);

    // Clear selection if the deleted tour was selected
    if (this.selectedTourSubject.getValue()?.tourId === tourId) {
      this.selectTour(null);
    }
  }
}
