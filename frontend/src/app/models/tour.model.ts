import { TransportType } from './enums.model';

export interface Tour {
  tourId: string;
  userId: string;
  name: string;
  description: string;
  startLocation: string;
  endLocation: string;
  startDate: Date | string;
  endDate: Date | string;
  transportType: TransportType;
  distance: number;
  estimatedTime: number;
  routeInformation: string;
  mapSnapshotPath: string;
  popularity: number;
  childfriendliness: number;
  imagePath?: string;
  createdAt?: Date | string;
  updatedAt?: Date | string;
}
