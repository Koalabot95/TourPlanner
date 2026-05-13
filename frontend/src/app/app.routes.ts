import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { TourPlanner } from './pages/tour-planner/tour-planner';
import { Logbook } from './pages/logbook/logbook';
import { Profile } from './pages/profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'dashboard', component: Dashboard },
  { path: 'tour-planner', component: TourPlanner },
  { path: 'logbook', component: Logbook },
  { path: 'profile', component: Profile },
];
