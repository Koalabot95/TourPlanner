import { Routes } from '@angular/router';
import { Home } from './views/pages/home/home';
import { Login } from './views/pages/login/login';
import { CreateTour } from './views/pages/create-tour/create-tour';
import { CreateTourLog } from './views/pages/create-tour-log/create-tour-log';
import { Profile } from './views/pages/profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'tour-planner', component: CreateTour },
  { path: 'logbook', component: CreateTourLog },
  { path: 'profile', component: Profile },
];
