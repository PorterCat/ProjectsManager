'use client';

import { useEffect, useState, useCallback } from 'react';
import { Space, Typography, Button, message } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { Project, ProjectFilterQuery } from '@/app/Models/Project';
import { projectService } from '@/app/Services/projectService';
import { ProjectList } from '@/app/Components/Projects/ProjectList';
import { ProjectFilters } from '@/app/Components/Projects/ProjectFilters';
import { CreateProjectModal } from '@/app/Components/Projects/CreateProjectModal';
import { ProjectDetailsModal } from '@/app/Components/Projects/ProjectDetailsModal';
import { Pagination } from '@/app/Components/Common/Pagination';
import { useRouter, useSearchParams } from 'next/navigation';
import styles from './projects.module.css';

const { Title } = Typography;

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalPages, setTotalPages] = useState(1);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [filters, setFilters] = useState<ProjectFilterQuery>({});
  
  const router = useRouter();
  const searchParams = useSearchParams();
  
  const urlPage = parseInt(searchParams.get('pageNum') || '1');
  const urlPageSize = parseInt(searchParams.get('pageSize') || '6');

  const [currentPage, setCurrentPage] = useState(urlPage);
  const [pageSize] = useState(urlPageSize);

  const loadProjects = useCallback(async (pageNum: number, filterQuery: ProjectFilterQuery = {}) => {
    try {
      setLoading(true);
      const data = await projectService.getProjectsByPage(pageNum, pageSize, filterQuery);
      
      setProjects(Array.isArray(data?.items) ? data.items : []);
      setTotalPages(data?.totalPages || 1);
      
      const params = new URLSearchParams();
      params.set('pageNum', pageNum.toString());
      params.set('pageSize', pageSize.toString());
      router.push(`/projects?${params.toString()}`, { scroll: false });
    } catch (error) {
      message.error('Ошибка загрузки проектов');
      console.error('Error while loading project:', error);
      setProjects([]);
    } finally {
      setLoading(false);
    }
  }, [pageSize, router]);

  useEffect(() => {
    loadProjects(currentPage, filters);
  }, [currentPage, filters, loadProjects]);

  const handleFiltersChange = (newFilters: ProjectFilterQuery) => {
    setFilters(newFilters);
    setCurrentPage(1);
  };

  const handleProjectCreated = async () => {
    setCreateModalOpen(false);
    message.success('Проект успешно создан');
    loadProjects(1, filters);
  };

  const handleViewDetails = (project: Project) => {
    setSelectedProject(project);
    setDetailsModalOpen(true);
  };

  const handleProjectUpdated = () => {
    loadProjects(currentPage, filters);
  };

  const handleProjectDeleted = () => {
    setDetailsModalOpen(false);
    loadProjects(currentPage, filters);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  return (
    <div className={styles.container}>
      <Space direction="vertical" size="large" style={{ width: '100%' }}>
        <div className={styles.header}>
          <Title level={2}>Проекты</Title>
          <Button 
            type="primary" 
            icon={<PlusOutlined />}
            onClick={() => setCreateModalOpen(true)}
          >
            Создать проект
          </Button>
        </div>

        <ProjectFilters 
          onFiltersChange={handleFiltersChange}
          loading={loading}
        />

        <ProjectList 
          projects={projects}
          loading={loading}
          onViewDetails={handleViewDetails}
        />
        
        {Array.isArray(projects) && projects.length > 0 && (
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        )}
      </Space>

      <CreateProjectModal 
        open={createModalOpen}
        onCancel={() => setCreateModalOpen(false)}
        onSuccess={handleProjectCreated}
      />

      <ProjectDetailsModal
        open={detailsModalOpen}
        project={selectedProject}
        onCancel={() => setDetailsModalOpen(false)}
        onSuccess={handleProjectUpdated}
        onDelete={handleProjectDeleted}
      />
    </div>
  );
}