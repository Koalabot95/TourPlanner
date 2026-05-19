import { Routes } from '@angular/router';

import { Login } from './views/pages/login/login';
import { Dashboard } from './views/pages/dashboard/dashboard';
import { TourPlanner } from './views/pages/tour-planner/tour-planner';
import { Logbook } from './views/pages/logbook/logbook';
import { Profile } from './views/pages/profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'dashboard', component: Dashboard },
  { path: 'tour-planner', component: TourPlanner },
  { path: 'logbook', component: Logbook },
  { path: 'profile', component: Profile },
];
