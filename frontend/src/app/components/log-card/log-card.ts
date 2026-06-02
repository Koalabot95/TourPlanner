import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TourCard } from '../tour-card/tour-card';
import { TourLog } from '../../models/tour-log.model';

@Component({
  selector: 'app-log-card',
  standalone: true,
  imports: [CommonModule, RouterLink, TourCard],
  templateUrl: './log-card.html',
  styleUrl: './log-card.scss',
})
export class LogCard {
  @Input() log!: TourLog;
  @Input() imageUrl: string = '';
  @Output() delete = new EventEmitter<string>();
}
