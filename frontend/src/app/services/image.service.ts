import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  getGlobalImages(): any[] {
    return JSON.parse(localStorage.getItem('global_images') || '[]');
  }

  getPreviewUrl(imagePath: string | undefined): string | null {
    if (!imagePath) return null;
    const imgRecord = this.getGlobalImages().find((img: any) => img.filename === imagePath);
    return imgRecord ? imgRecord.data : null;
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
