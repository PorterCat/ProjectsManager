'use client';

import { useState } from 'react';
import { 
  Form, 
  Input, 
  DatePicker, 
  InputNumber, 
  Select, 
  Button, 
  Space,
  Row,
  Col,
  Collapse
} from 'antd';
import { 
  FilterOutlined, 
  SortAscendingOutlined, 
  ClearOutlined,
  DownOutlined,
  UpOutlined 
} from '@ant-design/icons';
import { ProjectFilterQuery } from '@/app/Models/Project';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;
const { Option } = Select;

interface ProjectFiltersProps {
  onFiltersChange: (filters: ProjectFilterQuery) => void;
  loading?: boolean;
}

export const ProjectFilters = ({ onFiltersChange, loading = false }: ProjectFiltersProps) => {
  const [form] = Form.useForm();
  const [expanded, setExpanded] = useState(false);

  const sortFields = [
    { label: 'Названию', value: 'title' },
    { label: 'Дате начала', value: 'startDate' },
    { label: 'Приоритету', value: 'priority' },
    { label: 'Заказчику', value: 'customerCompanyName' },
    { label: 'Исполнителю', value: 'contractorCompanyName' },
  ];

  const handleSubmit = (values: any) => {
    const filters: ProjectFilterQuery = {};

    if (values.searchText) {
      filters.searchText = values.searchText;
    }

    if (values.dateRange && values.dateRange[0] && values.dateRange[1]) {
      filters.startDateFrom = values.dateRange[0].toDate();
      filters.startDateTo = values.dateRange[1].toDate();
    }

    if (values.priorityFrom !== undefined && values.priorityFrom !== null) {
      filters.priorityFrom = values.priorityFrom;
    }
    if (values.priorityTo !== undefined && values.priorityTo !== null) {
      filters.priorityTo = values.priorityTo;
    }

    if (values.sortBy) {
      filters.sortBy = values.sortBy;
      filters.sortDescending = values.sortOrder === 'desc';
    }

    onFiltersChange(filters);
  };

  const handleReset = () => {
    form.resetFields();
    onFiltersChange({});
  };

  const toggleExpanded = () => {
    setExpanded(!expanded);
  };

  return (
    <div style={{ 
      background: '#fafafa', 
      padding: '16px', 
      borderRadius: '8px',
      border: '1px solid #d9d9d9',
      marginBottom: '16px'
    }}>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        disabled={loading}
      >
        <Row gutter={[12, 12]} align="bottom">
          <Col xs={24} sm={12} md={6}>
            <Form.Item name="searchText" style={{ marginBottom: 0 }}>
              <Input 
                placeholder="Поиск проектов..." 
                allowClear
                size="small"
              />
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={4}>
            <Form.Item name="sortBy" style={{ marginBottom: 0 }}>
              <Select 
                placeholder="Сортировка" 
                size="small"
                allowClear
              >
                {sortFields.map(field => (
                  <Option key={field.value} value={field.value}>
                    {field.label}
                  </Option>
                ))}
              </Select>
            </Form.Item>
          </Col>

          <Col xs={12} sm={6} md={3}>
            <Form.Item name="sortOrder" style={{ marginBottom: 0 }}>
              <Select placeholder="Порядок" size="small">
                <Option value="asc">По возрастанию</Option>
                <Option value="desc">По убыванию</Option>
              </Select>
            </Form.Item>
          </Col>

          <Col xs={24} sm={12} md={11}>
            <Space style={{ width: '100%', justifyContent: 'flex-end' }}>
              <Button 
                type="link" 
                size="small"
                onClick={toggleExpanded}
                icon={expanded ? <UpOutlined /> : <DownOutlined />}
                style={{ padding: '0 8px' }}
              >
                {expanded ? 'Скрыть' : 'Фильтры'}
              </Button>
              
              <Button 
                type="primary" 
                htmlType="submit" 
                size="small"
                icon={<SortAscendingOutlined />}
                loading={loading}
              >
                Применить
              </Button>
              
              <Button 
                size="small"
                onClick={handleReset}
                icon={<ClearOutlined />}
                disabled={loading}
              >
                Сбросить
              </Button>
            </Space>
          </Col>
        </Row>

        {expanded && (
          <div style={{ marginTop: '16px', paddingTop: '16px', borderTop: '1px solid #e8e8e8' }}>
            <Row gutter={[12, 12]}>
              <Col xs={24} md={12} lg={8}>
                <Form.Item label="Диапазон дат" name="dateRange" style={{ marginBottom: 0 }}>
                  <RangePicker 
                    style={{ width: '100%' }}
                    format="DD.MM.YYYY"
                    placeholder={['Дата от', 'Дата до']}
                    size="small"
                    allowEmpty={[true, true]}
                  />
                </Form.Item>
              </Col>
              
              <Col xs={12} md={6} lg={4}>
                <Form.Item label="Приоритет от" name="priorityFrom" style={{ marginBottom: 0 }}>
                  <InputNumber 
                    min={1}
                    max={10}
                    placeholder="Мин"
                    style={{ width: '100%' }}
                    size="small"
                  />
                </Form.Item>
              </Col>
              
              <Col xs={12} md={6} lg={4}>
                <Form.Item label="Приоритет до" name="priorityTo" style={{ marginBottom: 0 }}>
                  <InputNumber 
                    min={1}
                    max={10}
                    placeholder="Макс"
                    style={{ width: '100%' }}
                    size="small"
                  />
                </Form.Item>
              </Col>
            </Row>
          </div>
        )}
      </Form>
    </div>
  );
};