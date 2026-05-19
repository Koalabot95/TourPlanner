export interface User {
  userId: string;
  username: string;
  passwordHash: string;
  email: string;
  createdAt: Date | string;
  firstName?: string;
  lastName?: string;
  bio?: string;
}
