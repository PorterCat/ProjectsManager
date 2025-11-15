import { Employee } from "./Employee";

export interface Project {
  id: string;
  title: string;
  startDate: Date;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  endDate?: Date;
  leaderId?: string;
}

export interface ProjectWithEmployeesResponse {
  id: string;
  title: string;
  startDate: Date;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  endDate?: Date;
  leaderId?: string;
  employees: Employee[];
}

export interface PatchProjectRequest {
  title?: string;
  customerCompanyName?: string;
  contractorCompanyName?: string;
  priority?: number;
  startDate?: string;
  endDate?: string;
  removeLeader?: boolean;
  leaderId?: string;
}

export interface CreateProjectRequest {
  title: string;
  startDate: string;
  endDate?: string;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  leaderId?: string;
  employeeIds?: string[];
}

export interface AssignLeaderRequest {
  leaderId: string;
}

export interface AssignEmployeesRequest {
  employeesToAdd: string[];
  employeesToRemove: string[];
}

export interface PageProjectsResponse {
  items: Project[];
  total: number;
  totalPages: number;
}

export interface ProjectFilterQuery {
  startDateFrom?: Date;
  startDateTo?: Date;
  priorityFrom?: number;
  priorityTo?: number;
  searchText?: string;
  sortBy?: string;
  sortDescending?: boolean;
}