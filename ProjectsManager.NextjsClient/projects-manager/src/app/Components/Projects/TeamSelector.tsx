'use client';

import { Card, Form, Select, Tag } from 'antd';
import { Employee } from '@/app/Models/Employee';
import { UserOutlined, TeamOutlined } from '@ant-design/icons';
import { FormInstance } from 'antd/es/form';

const { Option } = Select;

interface TeamSelectorsProps {
  employees: Employee[];
  loading: boolean;
  onSearch: (searchText?: string) => void;
  form?: FormInstance;
}

export const LeaderSelector = ({ employees, loading, onSearch }: TeamSelectorsProps) => (
  <Card title="Руководитель проекта">
    <Form.Item
      label="Выберите руководителя"
      name="leaderId"
    >
      <Select
        showSearch
        placeholder="Поиск руководителя"
        size="large"
        loading={loading}
        onSearch={onSearch}
        filterOption={false}
        optionFilterProp="children"
        suffixIcon={<UserOutlined />}
        allowClear
        notFoundContent="Сотрудники не найдены"
        optionLabelProp="label"
        labelInValue={false}
        style={{ width: '100%' }}
      >
        <Option value={undefined} key="no-leader" label="Без руководителя">
          Без руководителя
        </Option>
        {Array.isArray(employees) && employees.length > 0 ? (
          employees.map(employee => {
            const fullName = `${employee.firstname} ${employee.lastname}`;
            return (
              <Option 
                key={employee.id}
                value={employee.id}
                label={fullName}
              >
                <div>
                  <div><strong>{fullName}</strong></div>
                  <div style={{ fontSize: '12px', color: '#666' }}>
                    {employee.email}
                  </div>
                </div>
              </Option>
            );
          })
        ) : (
          !loading && <Option disabled key="no-data">Сотрудники не найдены</Option>
        )}
      </Select>
    </Form.Item>
  </Card>
);

export const EmployeesSelector = ({ employees, loading, onSearch, form }: TeamSelectorsProps) => {
  const leaderId = form?.getFieldValue('leaderId');
  
  const getFilteredEmployeesForPerformers = () => {
    if (!leaderId) {
      return employees;
    }
    return employees.filter(employee => employee.id !== leaderId);
  };

  return (
    <Card title="Участники команды">
      <Form.Item
        label="Выберите исполнителей"
        name="performerIds"
      >
        <Select
          mode="multiple"
          showSearch
          placeholder="Поиск исполнителей"
          size="large"
          loading={loading}
          onSearch={onSearch}
          filterOption={false}
          optionFilterProp="children"
          suffixIcon={<TeamOutlined />}
          notFoundContent="Сотрудники не найдены"
          optionLabelProp="label"
          labelInValue={false}
          style={{ width: '100%' }}
          tagRender={(props) => (
            <Tag
              closable={props.closable}
              onClose={props.onClose}
              style={{ margin: '2px 4px' }}
            >
              {props.label}
            </Tag>
          )}
        >
          {Array.isArray(employees) && employees.length > 0 ? (
            getFilteredEmployeesForPerformers().map(employee => {
              const fullName = `${employee.firstname} ${employee.lastname}`;
              return (
                <Option 
                  key={employee.id}
                  value={employee.id}
                  label={fullName}
                >
                  <div>
                    <div><strong>{fullName}</strong></div>
                    <div style={{ fontSize: '12px', color: '#666' }}>
                      {employee.email}
                    </div>
                  </div>
                </Option>
              );
            })
          ) : (
            !loading && <Option disabled key="no-data">Сотрудники не найдены</Option>
          )}
        </Select>
      </Form.Item>
    </Card>
  );
};