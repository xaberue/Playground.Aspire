import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppointmentModel } from './appointment.model';

@Injectable({
  providedIn: 'root',
})
export class AppointmentApiService {
  private readonly apiUrl = 'https://localhost:7159/api/appointments';

  constructor(private http: HttpClient) { }

  getAppointments(): Observable<AppointmentModel[]> {
    return this.http.get<AppointmentModel[]>(`${this.apiUrl}/current`);
  }
}
