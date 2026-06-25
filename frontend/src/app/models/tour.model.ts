import { TransportMode } from './enums.model';

export interface Tour {
  tourId?: string;         
  userId: string;
  name: string;
  description?: string;    
  startLocation: string;
  endLocation: string;
  startDate: string | Date; 
  endDate: string | Date;  
  //transportType: TransportMode;
  transportType: string; 
  distance?: number;
  estimatedTime?: number;
  routeInformation?: string;
  mapSnapshotPath?: string;
  popularity: number;
  childFriendliness: number;
  createdAt?: string | Date;
  updatedAt?: string | Date;
}
