import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Login } from './pages/login/login';
import { CreateTour } from './pages/create-tour/create-tour';
import { CreateTourLog } from './pages/create-tour-log/create-tour-log';
import { Profile } from './pages/profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'create-tour', component: CreateTour },
  { path: 'create-tour-log', component: CreateTourLog },
  { path: 'profile', component: Profile },
];
