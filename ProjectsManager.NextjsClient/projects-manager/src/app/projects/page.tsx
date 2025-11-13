'use client';

import { useEffect, useState } from 'react';
import { Button, InputNumber, Space, Typography, message } from 'antd';
import { Project } from '@/app/Models/Project';
import { projectService } from '@/app/Services/projectService';
import { LeftOutlined, PlusOutlined, RightOutlined } from '@ant-design/icons';
import { ProjectList } from '@/app/Components/Projects/ProjectList';
import { CreateProjectModal } from '@/app/Components/Projects/CreateProjectModal';
import { useTranslation } from 'react-i18next';
import { useRouter, useSearchParams } from 'next/navigation';

const { Title } = Typography;

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalPages, setTotalPages] = useState(1);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  
  const router = useRouter();
  const searchParams = useSearchParams();
  
  const urlPage = parseInt(searchParams.get('pageNum') || '1');
  const urlPageSize = parseInt(searchParams.get('pageSize') || '6');

  const [currentPage, setCurrentPage] = useState(urlPage);
  const [pageSize] = useState(urlPageSize);

  const { t } = useTranslation('projects');

  const loadProjects = async (pageNum: number, pageSize: number) => {
    try {
      setLoading(true);
      const data = await projectService.getProjectsByPage(pageNum, pageSize);
      setProjects(data.items);
      setTotalPages(data.totalPages);
      
      if (pageNum > data.totalPages && data.totalPages > 0) {
        const correctedPage = data.totalPages;
        setCurrentPage(correctedPage);
        updateUrl(correctedPage);
      }
    } catch (error) {
      message.error(t('projects.errorLoading'));
      console.error('Error loading projects:', error);
    } finally {
      setLoading(false);
    }
  };

  const updateUrl = (pageNum: number) => {
    const params = new URLSearchParams();
    params.set('pageNum', pageNum.toString());
    params.set('pageSize', pageSize.toString());
    router.push(`/projects?${params.toString()}`, { scroll: false });
  };

  useEffect(() => {
    if (currentPage > totalPages && totalPages > 0) {
      const correctedPage = totalPages;
      setCurrentPage(correctedPage);
      updateUrl(correctedPage);
      return;
    }
    
    loadProjects(currentPage, pageSize);
    updateUrl(currentPage);
  }, [currentPage, pageSize, totalPages]);

  const handleProjectCreated = () => {
    setCreateModalOpen(false);
    setCurrentPage(1);
    updateUrl(1);
  };

  const handlePageChange = (value: number | null) => {
    if (value && value >= 1 && value <= totalPages) {
      setCurrentPage(value);
    }
  };

  const prevPage = () => currentPage > 1 && setCurrentPage(currentPage - 1);
  const nextPage = () => currentPage < totalPages && setCurrentPage(currentPage + 1);

  return (
    <div className="projects">
      <Space direction="vertical" style={{ width: '100%' }} size="large">
        <div className="projects-header">
          <Title level={2}>{t('title')}</Title>
          <Button 
            type="primary" 
            icon={<PlusOutlined />}
            onClick={() => setCreateModalOpen(true)}
          >
            {t('createProject')}
          </Button>
        </div>

        <ProjectList 
          projects={projects}
          loading={loading}
          onViewDetails={(project) => console.log('Project details:', project)}
        />
        
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <Button 
            icon={<LeftOutlined />}
            onClick={prevPage}
            disabled={currentPage === 1}
          />

          <Space.Compact>
            <InputNumber
              min={1}
              max={totalPages}
              size="small"
              value={currentPage}
              onChange={handlePageChange}
              controls={false}
              style={{ width: 60 }}
            />
            <span style={{ 
              padding: '0 8px', 
              display: 'flex', 
              alignItems: 'center', 
              background: '#fafafa',
              border: '1px solid #d9d9d9',
              borderLeft: 'none'
            }}>
              / {totalPages}
            </span>
          </Space.Compact>

          <Button 
            icon={<RightOutlined />}
            onClick={nextPage}
            disabled={currentPage === totalPages}
          />
        </div>
      </Space>

      <CreateProjectModal 
        open={createModalOpen}
        onCancel={() => setCreateModalOpen(false)}
        onSuccess={handleProjectCreated}
      />
    </div>
  );
}