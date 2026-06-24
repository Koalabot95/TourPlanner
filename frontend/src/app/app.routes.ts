import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Register } from './pages/register/register';
import { Login } from './pages/login/login';
import { CreateTour } from './pages/create-tour/create-tour';
import { CreateTourLog } from './pages/create-tour-log/create-tour-log';
import { Profile } from './pages/profile/profile';
import { TourDetails } from './pages/tour-details/tour-details';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'register', component: Register },
  { path: 'login', component: Login },
  { path: 'create-tour', component: CreateTour },
  { path: 'edit-tour/:id', component: CreateTour },
  { path: 'tour-details/:id', component: TourDetails },
  { path: 'create-tour-log', component: CreateTourLog },
  { path: 'edit-tour-log/:id', component: CreateTourLog },
  { path: 'profile', component: Profile },
];
