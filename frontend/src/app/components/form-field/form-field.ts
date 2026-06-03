import { Component, ContentChild, Input } from '@angular/core';
import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-form-field',
  imports: [],
  templateUrl: './form-field.html',
  styleUrl: './form-field.scss',
})
export class FormField {
  @Input() label: string = '';
  @Input() errorMessage: string = '';
  @ContentChild(NgModel) control?: NgModel;
}
