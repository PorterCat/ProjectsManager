'use client';

import { Card, Button, Space, Typography } from 'antd';
import { Project } from '@/app/Models/Project';
import { useTranslation } from 'react-i18next';

const { Text } = Typography;

interface ProjectCardProps {
  project: Project;
  onViewDetails: (project: Project) => void;
}

export const ProjectCard = ({ project, onViewDetails }: ProjectCardProps) => {
  const { t } = useTranslation('projects');

  return (
    <Card
      title={project.title}
      style={{ height: '100%' }}
      hoverable
      actions={[
        <Button 
          type="link" 
          key="view"
          onClick={() => onViewDetails(project)}
        >
          {t('projectDetails')}
        </Button>,
      ]}
    >
      <Space direction="vertical" style={{ width: '100%' }}>
        <div>
          <Text strong>ID: </Text>
          <Text type="secondary" style={{ fontSize: '12px' }}>
            {project.id.toString().toLocaleUpperCase()}
          </Text>
        </div>
        <div>
          <Text strong>{t('startDate')}: </Text>
          <Text>{project.startDate.toString()}</Text>
        </div>
        {project.endDate && (
          <div>
            <Text strong>{t('endDate')}: </Text>
            <Text>{project.endDate.toString()}</Text>
          </div>
        )}
        {project.customerCompanyName && (
          <div>
            <Text strong>{t('customer')}: </Text>
            <Text>{project.customerCompanyName}</Text>
          </div>
        )}
      </Space>
    </Card>
  );
};