export interface AppointmentUpdatedModel {
  id: string;
  code: string;
  box: string;
  status: string;
}


export interface AppointmentUpdatedModelEntry {
  model: AppointmentUpdatedModel;
  highlighted?: boolean;
}
