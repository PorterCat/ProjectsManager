'use client';

import { List, Typography, Empty } from 'antd';
import { Project } from '@/app/Models/Project';
import { ProjectCard } from './ProjectCard';
import { useTranslation } from 'react-i18next';

const { Text } = Typography;

interface ProjectListProps {
  projects: Project[];
  loading: boolean;
  onViewDetails: (project: Project) => void;
}

export const ProjectList = ({ projects, loading, onViewDetails }: ProjectListProps) => {
  const { t } = useTranslation('projects');

  return (
    <List
      loading={loading}
      grid={{
        gutter: 16,
        xs: 1,
        sm: 2,
        md: 2,
        lg: 3,
        xl: 3,
        xxl: 4,
      }}
      dataSource={projects}
      renderItem={(project) => (
        <List.Item>
          <ProjectCard 
            project={project} 
            onViewDetails={onViewDetails}
          />
        </List.Item>
      )}
      locale={{ 
        emptyText: (
          <Empty 
            description={t('notFound')} 
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          />
        ) 
      }}
    />
  );
};