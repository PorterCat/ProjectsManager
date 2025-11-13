export interface Project {
  id: string;
  title: string;
  startDate: Date;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  endDate?: Date | null;
}

export interface PageProjectsResponse {
  items: Project[];
  total: number;
  totalPages: number;
}

export interface CreateProjectRequest {
  Title: string;
  StartDate: string;
  EndDate?: string | null;
  Priority: number;
  CustomerCompanyName: string;
  ContractorCompanyName: string;
  LeaderId?: string | null;
}