import { Component } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile',
  imports: [Navbar, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile {}
