import { Component } from '@angular/core';
import { Navbar } from '../../components/navbar/navbar';

@Component({
  selector: 'app-logbook',
  imports: [Navbar],
  templateUrl: './logbook.html',
  styleUrl: './logbook.scss',
})
export class Logbook {}
