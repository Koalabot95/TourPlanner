import { TransportType } from './enums.model';

export interface Tour {
  tourId: string;
  userId: string;
  name: string;
  description: string;
  startLocation: string;
  endLocation: string;
  transportType: TransportType;
  distance: number;
  estimatedTime: number;
  mapSnapshotPath: string;
  popularity: number;
  imagePath: string;
}
