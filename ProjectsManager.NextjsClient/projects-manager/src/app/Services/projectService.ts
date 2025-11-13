import { Project, CreateProjectRequest, PageProjectsResponse } from '@/app/Models/Project';
import { api } from './api';

export const projectService = {
  async getAllProjects(): Promise<Project[]> {
    const { data } = await api.get<Project[]>('/project/all');
    return data;
  },

  async getProjectById(id: string): Promise<Project> {
    const { data } = await api.get<Project>(`/project/${id}`);
    return data;
  },

  async getProjectsByPage(pageNum: number, pageSize: number): Promise<PageProjectsResponse> {
    const { data } = await api.get<PageProjectsResponse>('/project/all', { 
      params: { pageNum, pageSize }
    });
    return data;
  },

  async createProject(projectData: CreateProjectRequest): Promise<BigInt> {
    const { data } = await api.post<BigInt>('/project', projectData);
    return data;
  },

  async deleteProject(id: string): Promise<Project> {
    const { data } = await api.delete<Project>(`/project/${id}`);
    return data;
  },
};