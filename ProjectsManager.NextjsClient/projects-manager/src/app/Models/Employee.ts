export interface Employee {
  id: string;
  firstname: string;
  lastname: string;
  patronymic?: string;
  email: string;
}

export interface CreateEmployeeRequest {
  firstname: string;
  lastname: string;
  patronymic?: string;
  email: string;
}

export interface UpdateEmployeeRequest {
  firstname?: string;
  lastname?: string;
  patronymic?: string;
  email?: string;
}

export interface EmployeeResponse {
  id: string;
  firstname: string;
  lastname: string;
  patronymic?: string;
  email: string;
}