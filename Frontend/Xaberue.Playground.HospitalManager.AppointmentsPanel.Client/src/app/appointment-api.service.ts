import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { AppointmentModel } from './appointment.model';
import { AppointmentUpdatedModel } from './appointment-updated.model';

@Injectable({
  providedIn: 'root',
})
export class AppointmentApiService {
  private readonly apiUrl = 'https://localhost:7159';
  private hubConnection: signalR.HubConnection;

  private appointmentsSubject = new BehaviorSubject<AppointmentModel[]>([]);
  public appointments$ = this.appointmentsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.apiUrl}/hub/appointment-updated`)
      .build();

    this.hubConnection.on('ReceiveAppointmentUpdated', (appointmentUpdated: AppointmentUpdatedModel) => {
      this.onAppointmentAdmission(appointmentUpdated);
    });

    this.hubConnection
      .start()
      .catch(err => console.error(err));
  }

  getAppointments(): void {
    this.http.get<AppointmentModel[]>(`${this.apiUrl}/api/appointments/current`)
      .subscribe(appointments => this.appointmentsSubject.next(appointments));
  }

  private onAppointmentAdmission(appointmentUpdated: AppointmentUpdatedModel): void {
    const appointments = this.appointmentsSubject.getValue();
    const index = appointments.findIndex(a => a.id === appointmentUpdated.id);

    if (index !== -1) {
      appointments[index] = { ...appointments[index], ...appointmentUpdated };
    } else {
      const appointment: AppointmentModel = {
        id: appointmentUpdated.id,
        code: appointmentUpdated.code,
        box: appointmentUpdated.box,
        status: appointmentUpdated.status,
      };
      appointments.push(appointment);
    }

    this.appointmentsSubject.next([...appointments]);
  }
}
