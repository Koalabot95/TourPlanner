import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TourLog } from '../models/tour-log.model';
import { TourViewModel } from './tour.viewmodel';

@Injectable({
  providedIn: 'root',
})
export class TourLogViewModel {
  private logsSubject = new BehaviorSubject<TourLog[]>([]);

  // Expose all logs
  allLogs$: Observable<TourLog[]> = this.logsSubject.asObservable();

  // Expose ONLY the logs for the currently selected tour
  selectedTourLogs$: Observable<TourLog[]>;

  constructor(private tourVm: TourViewModel) {
    // Automatically filter logs whenever the selected tour changes
    this.selectedTourLogs$ = this.tourVm.selectedTour$.pipe(
      map((selectedTour) => {
        if (!selectedTour) return [];
        return this.logsSubject.getValue().filter((log) => log.tourId === selectedTour.tourId);
      }),
    );
  }

  // --- Actions triggered by the View ---

  addLog(newLog: TourLog): void {
    const currentLogs = this.logsSubject.getValue();
    this.logsSubject.next([...currentLogs, newLog]);
    // Force the selectedTourLogs$ observable to emit new filtered data
    this.tourVm.selectTour(this.tourVm['selectedTourSubject'].getValue());
  }

  updateLog(updatedLog: TourLog): void {
    const currentLogs = this.logsSubject.getValue();
    const index = currentLogs.findIndex((l) => l.logId === updatedLog.logId);
    if (index !== -1) {
      currentLogs[index] = updatedLog;
      this.logsSubject.next([...currentLogs]);
      this.tourVm.selectTour(this.tourVm['selectedTourSubject'].getValue());
    }
  }

  deleteLog(logId: string): void {
    const currentLogs = this.logsSubject.getValue().filter((l) => l.logId !== logId);
    this.logsSubject.next(currentLogs);
    this.tourVm.selectTour(this.tourVm['selectedTourSubject'].getValue());
  }
}
