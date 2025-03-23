import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterOutlet } from '@angular/router';
import { AppointmentModel } from './appointment.model';
import { AppointmentApiService } from './appointments-api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatCardModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  appointments: AppointmentModel[] = [];

  constructor(private appointmentService: AppointmentApiService) { }

  ngOnInit(): void {
    this.appointmentService.getAppointments().subscribe(appointments => {
      this.appointments = appointments;
    });
  }

}
