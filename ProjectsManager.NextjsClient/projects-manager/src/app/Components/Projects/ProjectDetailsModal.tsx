'use client';

import { Modal, Form, Input, DatePicker, Flex, Button, message, Select, Card, Tag, Space, Divider, Spin, Tabs } from 'antd';
import { Project, ProjectWithEmployeesResponse, PatchProjectRequest, AssignLeaderRequest, AssignEmployeesRequest } from '@/app/Models/Project';
import { Employee } from '@/app/Models/Employee';
import { projectService } from "@/app/Services/projectService";
import { employeeService } from "@/app/Services/employeeService";
import dayjs from 'dayjs';
import { useState, useEffect, useRef } from 'react';
import { EditOutlined, DeleteOutlined, SaveOutlined, CloseOutlined, UserOutlined, TeamOutlined } from '@ant-design/icons';

const { Option } = Select;
const { TabPane } = Tabs;

interface ProjectDetailsModalProps {
  open: boolean;
  project: Project | null;
  onCancel: () => void;
  onSuccess: () => void;
  onDelete: (projectId: string) => void;
}

export const ProjectDetailsModal = ({ open, project, onCancel, onSuccess, onDelete }: ProjectDetailsModalProps) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [projectWithEmployees, setProjectWithEmployees] = useState<ProjectWithEmployeesResponse | null>(null);
  const [basicProjectData, setBasicProjectData] = useState<Project | null>(null);
  const [searchLoading, setSearchLoading] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [dataLoading, setDataLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('basic');
  
  const originalTeamStateRef = useRef<{
    leaderId?: string;
    employeeIds: string[];
  }>({ employeeIds: [] });

  const currentLeaderId = Form.useWatch('leaderId', form);

  useEffect(() => {
    if (open && project) {
      loadAllProjectData(project.id);
      loadEmployees();
    }
  }, [open, project]);

  const loadAllProjectData = async (projectId: string) => {
    try {
      setDataLoading(true);
      
      const [basicData, employeesData] = await Promise.all([
        projectService.getProjectById(projectId),
        projectService.getProjectWithEmployees(projectId)
      ]);
      
      setBasicProjectData(basicData);
      setProjectWithEmployees(employeesData);
      
      const currentEmployeeIds = employeesData.employees?.map(e => e.id) || [];
      
      originalTeamStateRef.current = {
        leaderId: employeesData.leaderId,
        employeeIds: currentEmployeeIds
      };
      
      const formData = {
        title: basicData.title,
        startDate: basicData.startDate ? dayjs(basicData.startDate) : null,
        endDate: basicData.endDate ? dayjs(basicData.endDate) : null,
        priority: basicData.priority || 1,
        customerCompanyName: basicData.customerCompanyName || '',
        contractorCompanyName: basicData.contractorCompanyName || '',
        leaderId: employeesData.leaderId || undefined,
        performerIds: currentEmployeeIds,
      };
      
      form.setFieldsValue(formData);
      setIsEditing(false);
    } catch (error) {
      console.error('Loading error:', error);
      message.error('Ошибка загрузки данных проекта');
    } finally {
      setDataLoading(false);
    }
  };

  const loadEmployees = async (searchText?: string) => {
    try {
      setSearchLoading(true);
      const employeeData = await employeeService.getAllEmployees(searchText);
      setEmployees(Array.isArray(employeeData) ? employeeData : []);
    } catch (error) {
      console.error('Error loading employees:', error);
      setEmployees([]);
    } finally {
      setSearchLoading(false);
    }
  };

  const getFilteredEmployeesForPerformers = () => {
    if (!currentLeaderId) {
      return employees;
    }
    return employees.filter(employee => employee.id !== currentLeaderId);
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      const values = await form.validateFields();
      
      if (!project) return;
      
      const updateData: PatchProjectRequest = {
        title: values.title,
        customerCompanyName: values.customerCompanyName,
        contractorCompanyName: values.contractorCompanyName,
        priority: values.priority,
        startDate: values.startDate?.format('YYYY-MM-DD'),
        endDate: values.endDate?.format('YYYY-MM-DD'),
      };
      
      await projectService.updateProject(project.id, updateData);

      await handleLeaderChanges(project.id, values.leaderId);

      await handleTeamChanges(project.id, values.performerIds);

      message.success('Проект успешно обновлен');
      setIsEditing(false);
      
      await loadAllProjectData(project.id);
      onSuccess();
    } catch (error: any) {
      console.error('Error updating project:', error);
      
      if (error.response?.data) {
        const errorData = error.response.data;
        console.error('Err body:', errorData);
        
        if (errorData.errors) {
          const errorMessages = Object.values(errorData.errors).flat().join(', ');
          message.error(`Ошибка валидации: ${errorMessages}`);
        } else if (errorData.title) {
          message.error(`Ошибка: ${errorData.title}`);
        } else if (typeof errorData === 'string') {
          message.error(`Ошибка: ${errorData}`);
        } else {
          message.error('Ошибка обновления проекта');
        }
      } else {
        message.error('Ошибка обновления проекта');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleLeaderChanges = async (projectId: string, newLeaderId?: string) => {
    const currentLeaderId = originalTeamStateRef.current.leaderId;

    
    if (newLeaderId !== currentLeaderId) {
      if (newLeaderId) {
        const assignLeaderRequest: AssignLeaderRequest = {
          leaderId: newLeaderId
        };
        await projectService.assignLeader(projectId, assignLeaderRequest);
      } else if (currentLeaderId) {
        await projectService.updateProject(projectId, { removeLeader: true });
      }
    }
  };

  const handleTeamChanges = async (projectId: string, newPerformerIds?: string[]) => {
    const currentEmployeeIds = originalTeamStateRef.current.employeeIds || [];
    const newEmployeeIds = newPerformerIds || [];
    
    const employeesToAdd = newEmployeeIds.filter((id: string) => !currentEmployeeIds.includes(id));
    const employeesToRemove = currentEmployeeIds.filter((id: string) => !newEmployeeIds.includes(id));
  

    if (employeesToAdd.length > 0 || employeesToRemove.length > 0) {
      const assignEmployeesRequest: AssignEmployeesRequest = {
        employeesToAdd,
        employeesToRemove
      };
      await projectService.assignEmployees(projectId, assignEmployeesRequest);
    } else {
      console.log('No command changes');
    }
  };

  const handleDelete = async () => {
    if (!project) return;
    
    Modal.confirm({
      title: 'Удаление проекта',
      content: 'Вы уверены, что хотите удалить этот проект?',
      okText: 'Удалить',
      cancelText: 'Отмена',
      okType: 'danger',
      onOk: async () => {
        try {
          await projectService.deleteProject(project.id);
          message.success('Проект успешно удален');
          onDelete(project.id);
          onCancel();
        } catch (error) {
          console.error('Error whild deleting:', error);
          message.error('Ошибка удаления проекта');
        }
      },
    });
  };

  const resetToOriginalValues = () => {
    if (basicProjectData && projectWithEmployees) {
      form.setFieldsValue({
        title: basicProjectData.title,
        startDate: basicProjectData.startDate ? dayjs(basicProjectData.startDate) : null,
        endDate: basicProjectData.endDate ? dayjs(basicProjectData.endDate) : null,
        priority: basicProjectData.priority,
        customerCompanyName: basicProjectData.customerCompanyName,
        contractorCompanyName: basicProjectData.contractorCompanyName,
        leaderId: projectWithEmployees.leaderId,
        performerIds: projectWithEmployees.employees?.map(e => e.id) || [],
      });
    }
  };

  const handleEdit = () => {
    if (projectWithEmployees) {
      originalTeamStateRef.current = {
        leaderId: projectWithEmployees.leaderId,
        employeeIds: projectWithEmployees.employees?.map(e => e.id) || []
      };
    }
    setIsEditing(true);
  };

  if (!project) return null;

  return (
    <Modal
      title={
        <Flex justify="space-between" align="center">
          <span>{isEditing ? 'Редактирование проекта' : project.title}</span>
          <Space>
            {!isEditing && !dataLoading && (
              <>
                <Button 
                  icon={<EditOutlined />} 
                  onClick={handleEdit}
                  size="small"
                >
                  Редактировать
                </Button>
                <Button 
                  danger 
                  icon={<DeleteOutlined />}
                  onClick={handleDelete}
                  size="small"
                >
                  Удалить
                </Button>
              </>
            )}
            {isEditing && (
              <>
                <Button 
                  icon={<CloseOutlined />}
                  onClick={() => {
                    resetToOriginalValues();
                    setIsEditing(false);
                  }}
                  size="small"
                >
                  Отмена
                </Button>
                <Button 
                  type="primary" 
                  icon={<SaveOutlined />}
                  loading={loading}
                  onClick={handleSave}
                  size="small"
                >
                  Сохранить
                </Button>
              </>
            )}
          </Space>
        </Flex>
      }
      open={open}
      onCancel={() => {
        setIsEditing(false);
        onCancel();
      }}
      footer={null}
      width={700}
      destroyOnHidden
      styles={{
        body: { 
          padding: '16px 0',
          maxHeight: '70vh',
          overflowY: 'auto'
        }
      }}
    >
      {dataLoading ? (
        <div style={{ textAlign: 'center', padding: '40px' }}>
          <Spin size="large" />
          <div style={{ marginTop: 12 }}>Загрузка данных проекта...</div>
        </div>
      ) : (
        <Form
          form={form}
          layout="vertical"
          disabled={!isEditing || dataLoading}
          size="small"
        >
        <Tabs 
          activeKey={activeTab} 
          onChange={setActiveTab}
          style={{ padding: '0 16px' }}
          items={[
            {
              key: 'basic',
              label: 'Основная информация',
              children: (
                <Flex gap="small" vertical>
                  <Card 
                    size="small"
                    styles={{ body: { padding: '12px' } }}
                  >
                    <Form.Item
                      label="Название проекта"
                      name="title"
                      rules={[{ required: true, message: 'Введите название проекта' }]}
                      style={{ marginBottom: 12 }}
                    >
                      <Input />
                    </Form.Item>

                    <Flex gap="small">
                      <Form.Item
                        label="Дата начала"
                        name="startDate"
                        rules={[{ required: true, message: 'Укажите дату начала' }]}
                        style={{ flex: 1, marginBottom: 12 }}
                      >
                        <DatePicker 
                          format="DD.MM.YYYY"
                          style={{ width: '100%' }}
                        />
                      </Form.Item>

                      <Form.Item
                        label="Дата окончания"
                        name="endDate"
                        style={{ flex: 1, marginBottom: 12 }}
                      >
                        <DatePicker 
                          format="DD.MM.YYYY"
                          style={{ width: '100%' }}
                        />
                      </Form.Item>

                      <Form.Item
                        label="Приоритет"
                        name="priority"
                        rules={[{ required: true, message: 'Выберите приоритет' }]}
                        style={{ flex: 0.8, marginBottom: 12 }}
                      >
                        <Select>
                          <Option value={1}>Низкий</Option>
                          <Option value={2}>Средний</Option>
                          <Option value={3}>Высокий</Option>
                        </Select>
                      </Form.Item>
                    </Flex>
                  </Card>
                </Flex>
              )
            },
            {
              key: 'companies',
              label: 'Компании',
              children: (
                <Flex gap="small" vertical>
                  <Card 
                    size="small"
                    styles={{ body: { padding: '12px' } }}
                  >
                    <Form.Item
                      label="Компания заказчика"
                      name="customerCompanyName"
                      rules={[{ required: true, message: 'Введите компанию заказчика' }]}
                      style={{ marginBottom: 12 }}
                    >
                      <Input />
                    </Form.Item>

                    <Form.Item
                      label="Компания исполнителя"
                      name="contractorCompanyName"
                      rules={[{ required: true, message: 'Введите компанию исполнителя' }]}
                      style={{ marginBottom: 12 }}
                    >
                      <Input />
                    </Form.Item>
                  </Card>
                </Flex>
              )
            },
            {
              key: 'team',
              label: 'Команда',
              children: (
                <Flex gap="small" vertical>
                  <Card 
                    title="Исполнители" 
                    size="small"
                    styles={{ body: { padding: '12px' } }}
                  >
                    <Form.Item
                      name="performerIds"
                      style={{ marginBottom: 12 }}
                    >
                      <Select
                        mode="multiple"
                        showSearch
                        placeholder="Поиск исполнителей"
                        loading={searchLoading}
                        onSearch={loadEmployees}
                        filterOption={false}
                        suffixIcon={<TeamOutlined />}
                        notFoundContent="Сотрудники не найдены"
                        tagRender={(props) => (
                          <Tag 
                            closable={props.closable} 
                            onClose={props.onClose}
                            style={{ margin: '1px 2px', fontSize: '12px' }}
                          >
                            {props.label}
                          </Tag>
                        )}
                      >
                        {getFilteredEmployeesForPerformers().map(employee => (
                          <Option key={employee.id} value={employee.id}>
                            {employee.firstname} {employee.lastname}
                          </Option>
                        ))}
                      </Select>
                    </Form.Item>
                  </Card>

                  <Card 
                    title="Руководитель проекта" 
                    size="small"
                    styles={{ body: { padding: '12px' } }}
                  >
                    <Form.Item
                      name="leaderId"
                      style={{ marginBottom: 12 }}
                    >
                      <Select
                        showSearch
                        placeholder="Руководитель не найден"
                        loading={searchLoading}
                        onSearch={loadEmployees}
                        filterOption={false}
                        suffixIcon={<UserOutlined />}
                        allowClear
                        notFoundContent="Сотрудники не найдены"
                      >
                        {employees.map(employee => (
                          <Option key={employee.id} value={employee.id}>
                            {employee.firstname} {employee.lastname} {employee.patronymic || ''}
                          </Option>
                        ))}
                      </Select>
                    </Form.Item>
                  </Card>

                  {!isEditing && projectWithEmployees?.employees && projectWithEmployees.employees.length > 0 && (
                    <Card 
                      title="Текущая команда" 
                      size="small"
                      styles={{ body: { padding: '12px' } }}
                    >
                      <Space wrap style={{ marginTop: 6 }} size={[4, 4]}>
                        {projectWithEmployees.employees.map(employee => (
                          <Tag 
                            key={employee.id} 
                            style={{ fontSize: '11px', margin: '2px' }}
                          >
                            {employee.firstname} {employee.lastname}
                            {projectWithEmployees.leaderId === employee.id && ' (Руководитель)'}
                          </Tag>
                        ))}
                      </Space>
                    </Card>
                  )}
                </Flex>
              )
            }
          ]}
        />
        </Form>
      )}
    </Modal>
  );
};