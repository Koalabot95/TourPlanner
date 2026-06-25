import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImageService {

  constructor(private http: HttpClient) { }

  getGlobalImages(): any[] {
    return JSON.parse(localStorage.getItem('global_images') || '[]');
  }

  getPreviewUrl(imagePath: string | undefined): string | null {
    if (!imagePath) return null;
    const imgRecord = this.getGlobalImages().find((img: any) => img.filename === imagePath);
    return imgRecord ? imgRecord.data : null;
  }

  uploadImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>('/api/image/upload', formData);
  }

  processAndSaveImage(
    selectedFile: File | null,
    previewUrl: string | null,
    currentImagePath: string | undefined,
    prefix: string,
  ): string | null {
    if (selectedFile && previewUrl) {
      const filename = `${prefix}-${Date.now()}-${selectedFile.name}`;
      const globalImages = this.getGlobalImages();
      globalImages.push({ filename: filename, data: previewUrl });

      try {
        localStorage.setItem('global_images', JSON.stringify(globalImages));
        return filename;
      } catch (e) {
        alert('Storage full! Could not save image.');
        return null; // Stop the save process
      }
    }
    // Return existing path if no new file was selected
    return currentImagePath || null;
  }
}
