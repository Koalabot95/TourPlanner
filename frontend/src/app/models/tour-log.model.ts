import { Difficulty } from './enums.model';
import { Image } from './image.model';

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
  createdAt?: Date | string;
  imageUrl?: Image;
}
