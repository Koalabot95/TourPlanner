import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [CommonModule],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
  // Controls the HTML button type (submit, button, or reset)
  @Input() type: 'button' | 'submit' | 'reset' = 'button';

  // Controls the base color class (e.g., 'primary' creates 'btn-primary')
  @Input() variant: 'primary' | 'secondary' | 'danger' = 'primary';
  
  // Allows you to pass extra utility classes like 'save-btn'
  @Input() customClass: string = '';
  
  // Disables the button
  @Input() disabled: boolean = false;

  // Emits the click event back to the parent component
  @Output() buttonClick = new EventEmitter<MouseEvent>();

  onClick(event: MouseEvent) {
    if (!this.disabled) {
      this.buttonClick.emit(event);
    }
  }
}
