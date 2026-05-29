import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-form-field',
  imports: [],
  templateUrl: './form-field.html',
  styleUrl: './form-field.scss',
})
export class FormField {
  @Input() label: string = '';
  @Input() showError: boolean | null = false;
  @Input() errorMessage: string = '';
}
