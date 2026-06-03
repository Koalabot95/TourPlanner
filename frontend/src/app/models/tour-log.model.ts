import { Difficulty } from './enums.model';

export interface TourLog {
  logId: string;
  tourId: string;
  name: string;
  dateTime: Date | string;
  comment: string;
  difficulty: Difficulty;
  totalDistance: number;
  totalTime: number;
  rating: number;
  imagePath: string;
}
