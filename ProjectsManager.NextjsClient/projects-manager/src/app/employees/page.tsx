'use client';

import { useEffect, useState, useCallback } from 'react';
import { Space, Typography, Button, Modal, message, Form } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { Employee, CreateEmployeeRequest, UpdateEmployeeRequest } from '@/app/Models/Employee';
import { employeeService } from '@/app/Services/employeeService';
import { EmployeeTable } from '@/app/Components/Employees/EmployeeTable';
import { EmployeeModal } from '@/app/Components/Employees/EmployeeModal';
import { EmployeeSearch } from '@/app/Components/Employees/EmployeeSearch';
import styles from './employees.module.css';

const { Title } = Typography;

export default function EmployeesPage() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editingEmployee, setEditingEmployee] = useState<Employee | null>(null);
  const [form] = Form.useForm();
  const [submitLoading, setSubmitLoading] = useState(false);

  const loadEmployees = useCallback(async (search?: string) => {
    try {
      setLoading(true);
      const data = await employeeService.getAllEmployees(search);
      setEmployees(Array.isArray(data) ? data : []);
    } catch (error) {
      message.error('Error loading employees');
      console.error('Ошибка загрузки сотрудников:', error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadEmployees();
  }, [loadEmployees]);

  const handleSearchChange = useCallback((value: string) => {
    setSearchText(value);
    const timeoutId = setTimeout(() => {
      loadEmployees(value);
    }, 300);
    
    return () => clearTimeout(timeoutId);
  }, [loadEmployees]);

  const handleCreate = () => {
    setEditingEmployee(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleEdit = (employee: Employee) => {
    setEditingEmployee(employee);
    setModalOpen(true);
    form.setFieldsValue({
      firstname: employee.firstname,
      lastname: employee.lastname,
      patronymic: employee.patronymic,
      email: employee.email,
    });
  };

  const handleDelete = (employee: Employee) => {
    Modal.confirm({
      title: 'Удаление сотрудника',
      content: `Вы уверены, что хотите удалить сотрудника ${employee.firstname} ${employee.lastname}?`,
      okText: 'Удалить',
      cancelText: 'Отмена',
      okType: 'danger',
      onOk: async () => {
        try {
          await employeeService.deleteEmployee(employee.id);
          message.success('Сотрудник успешно удален');
          loadEmployees(searchText);
        } catch (error) {
          console.error('Error deleting emploees:', error);
          message.error('Ошибка удаления сотрудника');
        }
      },
    });
  };

  const handleSubmit = async () => {
    try {
      setSubmitLoading(true);
      const values = await form.validateFields();
      
      if (editingEmployee) {
        const updateData: UpdateEmployeeRequest = {
          firstname: values.firstname,
          lastname: values.lastname,
          patronymic: values.patronymic,
          email: values.email,
        };
        await employeeService.updateEmployee(editingEmployee.id, updateData);
        message.success('Сотрудник успешно обновлен');
      } else {
        const createData: CreateEmployeeRequest = {
          firstname: values.firstname,
          lastname: values.lastname,
          patronymic: values.patronymic,
          email: values.email,
        };
        await employeeService.createEmployee(createData);
        message.success('Сотрудник успешно создан');
      }
      
      setModalOpen(false);
      form.resetFields();
      loadEmployees(searchText);
    } catch (error) {
      console.error('Ошибка сохранения сотрудника:', error);
      message.error('Ошибка сохранения сотрудника');
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleModalCancel = () => {
    setModalOpen(false);
    form.resetFields();
  };

  return (
    <div className={styles.container}>
      <Space direction="vertical" size="middle" style={{ width: '100%' }}>
        <div className={styles.header}>
          <Title level={2}>Сотрудники</Title>
          <Button 
            type="primary" 
            icon={<PlusOutlined />}
            onClick={handleCreate}
          >
            Добавить сотрудника
          </Button>
        </div>

        <EmployeeSearch 
          value={searchText}
          onChange={handleSearchChange}
        />

        <EmployeeTable
          employees={employees}
          loading={loading}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      </Space>

      <EmployeeModal
        open={modalOpen}
        editingEmployee={editingEmployee}
        loading={submitLoading}
        onCancel={handleModalCancel}
        onSubmit={handleSubmit}
        form={form}
      />
    </div>
  );
}