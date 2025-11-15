import { Input } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import styles from '@/app/employees/employees.module.css';

interface EmployeeSearchProps {
  value: string;
  onChange: (value: string) => void;
}

export const EmployeeSearch: React.FC<EmployeeSearchProps> = ({
  value,
  onChange,
}) => {
  return (
    <Input
      placeholder="Поиск по ФИО"
      allowClear
      prefix={<SearchOutlined />}
      size="large"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className={styles.search}
    />
  );
};