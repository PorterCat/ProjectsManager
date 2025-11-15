import { Table, Space, Button } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { Employee } from '@/app/Models/Employee';

interface EmployeeTableProps {
  employees: Employee[];
  loading: boolean;
  onEdit: (employee: Employee) => void;
  onDelete: (employee: Employee) => void;
}

export const EmployeeTable: React.FC<EmployeeTableProps> = ({
  employees,
  loading,
  onEdit,
  onDelete,
}) => {
  const columns = [
    {
      key: 'fio',
      render: (_: any, employee: Employee) => (
        <div>
          <div style={{ fontWeight: 500 }}>
            {employee.lastname} {employee.firstname} {employee.patronymic || ''}
          </div>
          <div style={{ color: '#666', fontSize: '12px' }}>
            {employee.email}
          </div>
        </div>
      ),
    },
    {
      key: 'actions',
      width: 100,
      render: (_: any, employee: Employee) => (
        <Space>
          <Button 
            type="link" 
            icon={<EditOutlined />}
            onClick={() => onEdit(employee)}
          />
          <Button 
            danger 
            type="link" 
            icon={<DeleteOutlined />}
            onClick={() => onDelete(employee)}
          />
        </Space>
      ),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={employees}
      loading={loading}
      rowKey="id"
      pagination={false}
      showHeader={false}
      scroll={{ x: 800 }}
    />
  );
};