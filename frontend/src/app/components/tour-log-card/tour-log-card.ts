import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TourCard } from '../tour-card/tour-card';
import { TourLog } from '../../models/tour-log.model';

@Component({
  selector: 'app-tour-log-card',
  standalone: true,
  imports: [CommonModule, RouterLink, TourCard],
  templateUrl: './tour-log-card.html',
  styleUrl: './tour-log-card.scss',
})
export class TourLogCard {
  @Input() log!: TourLog;
  @Input() imageUrl: string = '';
  @Output() delete = new EventEmitter<string>();
}
