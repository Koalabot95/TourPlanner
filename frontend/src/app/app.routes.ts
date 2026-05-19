import { Routes } from '@angular/router';
import { Home } from './views/pages/home/home';
import { Login } from './views/pages/login/login';
import { TourPlanner } from './views/pages/tour-planner/tour-planner';
import { Logbook } from './views/pages/logbook/logbook';
import { Profile } from './views/pages/profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'tour-planner', component: TourPlanner },
  { path: 'logbook', component: Logbook },
  { path: 'profile', component: Profile },
];
