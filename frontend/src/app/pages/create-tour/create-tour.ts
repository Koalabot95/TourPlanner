import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Tour } from '../../models/tour.model';
import { TransportType } from '../../models/enums.model';

@Component({
  selector: 'app-create-tour',
  imports: [Navbar, Button, FormsModule],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.scss',
})
export class CreateTour {
  // The ViewModel State
  tour: Tour = {
    userId: 'default-user-id', // Placeholder user ID, replace with actual user management logic
    tourId: 'tour-' + Date.now(), // Generate a unique tour ID based on timestamp
    name: '',
    description: '',
    startLocation: '',
    endLocation: '',
    startDate: '',
    endDate: '',
    transportType: TransportType.Hike, // Default transport type
    distance: 0,
    estimatedTime: 0,
    routeInformation: '',
    mapSnapshotPath: '',
    popularity: 0,
    childfriendliness: 0,
  };

  constructor(private router: Router) {} // Inject Router

  saveTour() {
    // 1. Get existing tours from local storage (or an empty array if none exist)
    const existingTours = JSON.parse(localStorage.getItem('tours') || '[]');

    // 2. Add the new tour to the array
    existingTours.push(this.tour);

    // 3. Save the updated array back to local storage
    localStorage.setItem('tours', JSON.stringify(existingTours));

    // 4. Redirect to home
    this.router.navigate(['/home']);
  }
}
