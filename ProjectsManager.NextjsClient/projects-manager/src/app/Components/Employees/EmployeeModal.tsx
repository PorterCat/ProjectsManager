import { Modal, Form, Input, Flex } from 'antd';
import { Employee, CreateEmployeeRequest, UpdateEmployeeRequest } from '@/app/Models/Employee';
import styles from '@/app/employees/employees.module.css';

interface EmployeeModalProps {
  open: boolean;
  editingEmployee: Employee | null;
  loading: boolean;
  onCancel: () => void;
  onSubmit: (values: any) => void;
  form: any;
}

export const EmployeeModal: React.FC<EmployeeModalProps> = ({
  open,
  editingEmployee,
  loading,
  onCancel,
  onSubmit,
  form,
}) => {
  return (
    <Modal
      title={editingEmployee ? 'Редактирование сотрудника' : 'Добавление сотрудника'}
      open={open}
      onCancel={onCancel}
      onOk={onSubmit}
      okText={editingEmployee ? 'Обновить' : 'Создать'}
      cancelText="Отмена"
      confirmLoading={loading}
      width={500}
      styles={{ body: { padding: '16px 0' } }}
    >
      <Form
        form={form}
        layout="vertical"
        name="employeeForm"
        size="small"
      >
        <Flex gap="small" vertical>
          <Form.Item
            name="firstname"
            label="Имя"
            rules={[{ required: true, message: 'Введите имя' }]}
            className={styles.formItem}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="lastname"
            label="Фамилия"
            rules={[{ required: true, message: 'Введите фамилию' }]}
            className={styles.formItem}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="patronymic"
            label="Отчество"
            className={styles.formItem}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: 'Введите email' },
              { type: 'email', message: 'Введите корректный email' }
            ]}
            className={styles.formItem}
          >
            <Input />
          </Form.Item>
        </Flex>
      </Form>
    </Modal>
  );
};