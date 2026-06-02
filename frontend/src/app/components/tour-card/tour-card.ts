import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-tour-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tour-card.html',
  styleUrl: './tour-card.scss',
})
export class TourCard {
  @Input() title: string = '';
  @Input() customClass: string = '';
  @Input() imageUrl: string = '';
  @Input() imageAlt: string = 'Card image';
}
