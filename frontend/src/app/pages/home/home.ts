import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';

@Component({
  selector: 'app-home',
  imports: [CommonModule, Navbar, RouterLink, Button],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})

/**
 * Home component serves as the landing page of the application. 
 * It displays a summary of tours and tour logs.
 * It reads data from local storage and binds it to the view for display. 
 * The component uses Angular's OnInit lifecycle hook to load data when the component is initialized.
 */
export class Home implements OnInit {
  // ViewModel State
  tours: any[] = [];
  tourLogs: any[] = [];

  // Runs automatically when the component loads
  ngOnInit() {
    this.loadData();
  }

  loadData() {
    // Read from local storage and assign to our ViewModel properties
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');
    this.tourLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
  }
}
