import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.scss',
})
export class ImageUpload {
  @Input() label: string = 'Image';
  @Input() isEditMode: boolean = false;
  @Input() previewUrl: string | null = null;
  @Input() submitted: boolean = false;

  @Output() fileSelected = new EventEmitter<{ file: File; preview: string }>();

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.fileSelected.emit({ file, preview: e.target.result });
      };
      reader.readAsDataURL(file);
    }
  }
}
