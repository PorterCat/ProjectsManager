import { 
  Project, 
  CreateProjectRequest, 
  PageProjectsResponse, 
  ProjectWithEmployeesResponse,
  PatchProjectRequest,
  AssignLeaderRequest,
  AssignEmployeesRequest,
  ProjectFilterQuery
} from '@/app/Models/Project';
import { api } from './api';

export const projectService = {
  async getAllProjects(pageQuery?: any, filterQuery?: ProjectFilterQuery): Promise<PageProjectsResponse> {
    const params = { 
      pageQuery, 
      filterQuery
    };
    const { data } = await api.get<PageProjectsResponse>('/project/all', { params });
    return data;
  },

  async getProjectById(id: string): Promise<Project> {
    const { data } = await api.get<Project>(`/project/${id}`);
    return data;
  },

  async getProjectWithEmployees(id: string): Promise<ProjectWithEmployeesResponse> {
    const { data } = await api.get<ProjectWithEmployeesResponse>(`/project/${id}/employees`);
    return data;
  },

  async getProjectsByPage(
    pageNum: number, 
    pageSize: number, 
    filterQuery?: ProjectFilterQuery
  ): Promise<PageProjectsResponse> {
    const params: any = { 
      pageNum, 
      pageSize, 
    };

    if (filterQuery) {
      if (filterQuery.startDateFrom) {
        params.startDateFrom = filterQuery.startDateFrom.toISOString().split('T')[0];
      }
      if (filterQuery.startDateTo) {
        params.startDateTo = filterQuery.startDateTo.toISOString().split('T')[0];
      }
      if (filterQuery.priorityFrom !== undefined) {
        params.priorityFrom = filterQuery.priorityFrom;
      }
      if (filterQuery.priorityTo !== undefined) {
        params.priorityTo = filterQuery.priorityTo;
      }
      if (filterQuery.searchText) {
        params.searchText = filterQuery.searchText;
      }
      if (filterQuery.sortBy) {
        params.sortBy = filterQuery.sortBy;
      }
      if (filterQuery.sortDescending !== undefined) {
        params.sortDescending = filterQuery.sortDescending;
      }
    }

    const { data } = await api.get<PageProjectsResponse>('/project/all', { params });
    return data;
  },

  async createProject(projectData: CreateProjectRequest): Promise<string> {
    const { data } = await api.post<string>('/project', projectData);
    return data;
  },

  async updateProject(id: string, projectData: PatchProjectRequest): Promise<void> {
    const { data } = await api.patch<void>(`/project/${id}`, projectData);
    return data;
  },

  async assignLeader(id: string, leaderData: AssignLeaderRequest): Promise<void> {
    const { data } = await api.patch<void>(`/project/${id}/leader`, leaderData);
    return data;
  },

  async assignEmployees(id: string, employeesData: AssignEmployeesRequest): Promise<void> {
    const { data } = await api.patch<void>(`/project/${id}/employees`, employeesData);
    return data;
  },

  async deleteProject(id: string): Promise<void> {
    const { data } = await api.delete<void>(`/project/${id}`);
    return data;
  }
};