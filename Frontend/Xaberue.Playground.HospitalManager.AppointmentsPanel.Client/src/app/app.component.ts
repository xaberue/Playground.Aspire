import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterOutlet } from '@angular/router';
import { AppointmentModel } from './appointment.model';
import { AppointmentApiService } from './appointment-api.service';
import { AppointmentUpdatedModelEntry } from './appointment-updated.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatCardModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'AppointmentsPanel';
  private previousAppointments: AppointmentUpdatedModelEntry[] = [];
  appointments: AppointmentUpdatedModelEntry[] = [];

  constructor(
    private appointmentService: AppointmentApiService)
  { }

  ngOnInit(): void {
    this.appointmentService.appointments$.subscribe(appointments => {
      const entries: AppointmentUpdatedModelEntry[] = [];
      appointments.forEach(appointment => {
        const isUpdatedOrCreated = !this.previousAppointments.some(
          prev => prev.model.code === appointment.code && prev.model.status === appointment.status && prev.model.box === appointment.box
        );
        const entry = <AppointmentUpdatedModelEntry>{
          model: appointment,
          highlighted: isUpdatedOrCreated
        };

        entries.push(entry);
      });

      this.appointments = entries;
      this.previousAppointments = this.appointments;
    });

    this.appointmentService.getAppointments();
  }

}
