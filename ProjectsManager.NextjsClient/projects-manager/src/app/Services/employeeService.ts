import { Employee, CreateEmployeeRequest, UpdateEmployeeRequest } from '@/app/Models/Employee';
import { api } from './api';

export const employeeService = {
  async getAllEmployees(search?: string): Promise<Employee[]> {
    try {
      const params = search ? { searchText: search } : {};
      const { data } = await api.get<Employee[]>('/employee/all', { params });
      return data;
    } catch (error) {
      console.error('Error fetching employees:', error);
      return [];
    }
  },

  async searchEmployees(searchText: string): Promise<Employee[]> {
    try {
      const { data } = await api.get<Employee[]>('/employee/all', { 
        params: { searchText }
      });
      return data;
    } catch (error) {
      console.error('Error searching employees:', error);
      return [];
    }
  },

  async getEmployeeById(id: string): Promise<Employee> {
    const { data } = await api.get<Employee>(`/employee/${id}`);
    return data;
  },

  async createEmployee(employeeData: CreateEmployeeRequest): Promise<string> {
    const { data } = await api.post<string>('/employee', employeeData); 
    return data;
  },

  async updateEmployee(id: string, employeeData: UpdateEmployeeRequest): Promise<void> {
    const { data } = await api.patch<void>(`/employee/${id}`, employeeData);
    return data;
  },

  async deleteEmployee(id: string): Promise<void> {
    const { data } = await api.delete<void>(`/employee/${id}`);
    return data;
  },
};