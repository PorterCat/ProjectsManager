import { Button, InputNumber, Space } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import styles from '@/app/projects/projects.module.css';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange,
}) => {
  const handlePageChange = (value: number | null) => {
    if (value && value >= 1 && value <= totalPages) {
      onPageChange(value);
    }
  };

  const prevPage = () => currentPage > 1 && onPageChange(currentPage - 1);
  const nextPage = () => currentPage < totalPages && onPageChange(currentPage + 1);

  return (
    <div className={styles.pagination}>
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
        <span className={styles.paginationCounter}>
          / {totalPages}
        </span>
      </Space.Compact>

      <Button 
        icon={<RightOutlined />}
        onClick={nextPage}
        disabled={currentPage === totalPages}
      />
    </div>
  );
};