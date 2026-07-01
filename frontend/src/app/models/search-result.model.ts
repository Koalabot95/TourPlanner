export interface SearchResult {
  tours: SearchTourDto[];
  totalCount: number;
}

export interface SearchTourDto {
  tourId: string;
  name: string;
  description?: string;
  startLocation: string;
  endLocation: string;
  transportType: string;
  distance?: number;
  estimatedTime?: number;
  popularity: number;
  childFriendliness: number;
  isFavorite: boolean;
}

export interface SearchLogResult {
  logs: SearchLogDto[];
  totalCount: number;
}

export interface SearchLogDto {
  logId: string;
  tourId: string;
  tourName?: string;
  name?: string;
  dateTime: string;
  difficulty?: string;
  totalDistance: number;
  totalTime: number;
  rating: number;
  comment?: string;
}