'use client';

import { Row, Col, Spin, Empty } from 'antd';
import { Project } from '@/app/Models/Project';
import { ProjectCard } from './ProjectCard';

interface ProjectListProps {
  projects: Project[];
  loading: boolean;
  onViewDetails: (project: Project) => void;
}

export const ProjectList = ({ projects, loading, onViewDetails }: ProjectListProps) => {
  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: '50px' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (projects.length === 0) {
    return (
      <Empty 
        description="Проекты не найдены" 
        image={Empty.PRESENTED_IMAGE_SIMPLE}
      />
    );
  }

  return (
    <Row gutter={[16, 16]}>
      {projects.map(project => (
        <Col key={project.id} xs={24} sm={12} lg={8}>
          <ProjectCard 
            project={project} 
            onViewDetails={onViewDetails}
          />
        </Col>
      ))}
    </Row>
  );
};