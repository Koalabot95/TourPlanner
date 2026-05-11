import { Routes } from '@angular/router';

import { Login } from './login/login';
import { Dashboard } from './dashboard/dashboard';
import { TourPlanner } from './tour-planner/tour-planner';
import { Logbook } from './logbook/logbook';
import { Profile } from './profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'dashboard', component: Dashboard },
  { path: 'tour-planner', component: TourPlanner },
  { path: 'logbook', component: Logbook },
  { path: 'profile', component: Profile },
];
