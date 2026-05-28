import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Navbar } from '../../components/navbar/navbar';
import { Button } from '../../components/button/button';
import { Tour } from '../../models/tour.model';
import { TourLog } from '../../models/tour-log.model';
import { Card } from '../../components/card/card';

@Component({
  selector: 'app-home',
  imports: [CommonModule, Navbar, RouterLink, Button, Card],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  tours: Tour[] = [];
  tourLogs: TourLog[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.tours = JSON.parse(localStorage.getItem('tours') || '[]');
    this.tourLogs = JSON.parse(localStorage.getItem('tourLogs') || '[]');
  }
}
