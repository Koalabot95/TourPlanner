export interface Image {
  imageId: string;
  logId: string;
  filePath: string;
  caption?: string;
  uploadedAt: Date | string;
}
