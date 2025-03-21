import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatCardModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  appointments = [
    <AppointmentModel>{ code: '1234', box: 'Box 1', state: 'Scheduled' },
    <AppointmentModel>{ code: '1235', box: 'Box 2', state: 'Scheduled' },
    <AppointmentModel>{ code: '1235', box: '-', state: 'Waiting' }
  ]


}


interface AppointmentModel {
  code: string;
  box: string;
  state: string;
}
