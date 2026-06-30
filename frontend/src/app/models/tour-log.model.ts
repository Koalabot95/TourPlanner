import { Difficulty } from './enums.model';

export interface TourLog {
  logId?: string;           
  tourId: string;
  name?: string;
  dateTime: Date | string;
  comment?: string;
  difficulty?: Difficulty; 
  totalDistance: number;
  totalTime: number;
  rating: number;
  createdAt?: string | Date;
}

export interface TourLogBackendDto {
  tourId: string;
  name?: string;
  dateTime: string;
  comment?: string;
  difficulty: string; // C#-DTO kompatibel
  totalDistance: number;
  totalTime: number;
  rating: number;
}
