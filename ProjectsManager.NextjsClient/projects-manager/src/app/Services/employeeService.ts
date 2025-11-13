import { Employee } from "@/app/Models/Employee";
import { api } from './api';

export const employeeService = {
  async getAllEmployees(): Promise<Employee[]> {
    const { data } = await api.get<Employee[]>('/employee/all');
    return data;
  },

  async searchEmployees(searchText: string): Promise<Employee[]> {
    const { data } = await api.get<Employee[]>('/employee/all', { 
      params: { searchText } 
    });
    return data;
  },
};