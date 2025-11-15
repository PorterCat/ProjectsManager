'use client';

import { Modal, Form, Input, DatePicker, Flex, Button, message, Steps, Upload, Card, Select } from 'antd';
import { CreateProjectRequest } from '@/app/Models/Project';
import { projectService } from "@/app/Services/projectService";
import { employeeService } from "@/app/Services/employeeService";
import dayjs from 'dayjs';
import { useState, useEffect, JSX } from 'react';
import { InboxOutlined } from '@ant-design/icons';
import { Employee } from '@/app/Models/Employee';
import { FormInstance } from 'antd/es/form';
import { RcFile, UploadFile } from 'antd/es/upload';
import { LeaderSelector, EmployeesSelector } from './TeamSelector';

const { Dragger } = Upload;
const { Option } = Select;

interface CreateProjectModalProps {
  open: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

export interface ProjectFormData {
  title: string;
  customerCompanyName: string;
  contractorCompanyName: string;
  startDate: dayjs.Dayjs;
  endDate?: dayjs.Dayjs;
  priority: number;
  leaderId?: string;
  performerIds?: string[];
  documents?: File[];
}

interface StepProps {
  form: FormInstance;
}

const BasicInfoStep = ({ form }: StepProps) => {
  const validateEndDate = (_: any, value: dayjs.Dayjs) => {
    const startDate = form.getFieldValue('startDate');
    
    if (!value || !startDate) {
      return Promise.resolve();
    }
    
    if (value.isBefore(startDate, 'day')) {
      return Promise.reject(new Error('Дата окончания не может быть раньше даты начала'));
    }
    
    return Promise.resolve();
  };

  return (
    <Flex gap="middle" vertical>
      <Form.Item
        label="Название проекта"
        name="title"
        rules={[
          { required: true, message: 'Введите название проекта' },
          { min: 3, message: 'Название должно содержать минимум 3 символа' }
        ]}
      >
        <Input 
          placeholder="Введите название проекта" 
          size="large"
        />
      </Form.Item>

      <Flex gap="middle">
        <Form.Item
          label="Дата начала"
          name="startDate"
          rules={[{ required: true, message: 'Укажите дату начала' }]}
          style={{ flex: 1 }}
        >
          <DatePicker 
            format="DD.MM.YYYY"
            placeholder="Выберите дату"
            style={{ width: '100%' }}
            size="large"
            onChange={() => {
              const endDate = form.getFieldValue('endDate');
              if (endDate) {
                form.validateFields(['endDate']);
              }
            }}
          />
        </Form.Item>

        <Form.Item
          label="Дата окончания"
          name="endDate"
          style={{ flex: 1 }}
          rules={[
            {
              validator: validateEndDate
            }
          ]}
          dependencies={['startDate']}
        >
          <DatePicker 
            format="DD.MM.YYYY"
            placeholder="Выберите дату"
            style={{ width: '100%' }}
            size="large"
          />
        </Form.Item>
      </Flex>

      <Form.Item
        label="Приоритет"
        name="priority"
        rules={[{ required: true, message: 'Выберите приоритет' }]}
      >
        <Select size="large" placeholder="Выберите приоритет">
          <Option value={1}>Низкий</Option>
          <Option value={2}>Средний</Option>
          <Option value={3}>Высокий</Option>
        </Select>
      </Form.Item>
    </Flex>
  );
};

const CompaniesStep = ({ form }: StepProps) => (
  <Flex gap="middle" vertical>
    <Form.Item
      label="Компания заказчика"
      name="customerCompanyName"
      rules={[{ required: true, message: 'Введите компанию заказчика' }]}
    >
      <Input 
        placeholder="Введите название компании" 
        size="large"
      />
    </Form.Item>

    <Form.Item
      label="Компания исполнителя"
      name="contractorCompanyName"
      rules={[{ required: true, message: 'Введите компанию исполнителя' }]}
    >
      <Input 
        placeholder="Введите название компании" 
        size="large"
      />
    </Form.Item>
  </Flex>
);

const DocumentsStep = ({ form }: StepProps) => {
  const [fileList, setFileList] = useState<UploadFile[]>([]);

  const handleBeforeUpload = (file: RcFile) => {
    const uploadFile: UploadFile = {
      uid: file.uid,
      name: file.name,
      size: file.size,
      type: file.type,
      lastModified: file.lastModified,
      lastModifiedDate: file.lastModifiedDate,
      status: 'done' as const,
      originFileObj: file,
    };

    setFileList(prevList => [...prevList, uploadFile]);
    
    const currentFiles = form.getFieldValue('documents') || [];
    const fileMetadata = {
      uid: file.uid,
      name: file.name,
      size: file.size,
      type: file.type,
    };
    form.setFieldValue('documents', [...currentFiles, fileMetadata]);
    
    return false;
  };

  const handleRemove = (file: UploadFile) => {
    setFileList(prevList => prevList.filter(f => f.uid !== file.uid));
    
    const currentFiles = form.getFieldValue('documents') || [];
    form.setFieldValue('documents', currentFiles.filter((f: any) => f.uid !== file.uid));
  };

  return (
    <Card title="Документы">
      <Form.Item name="documents">
        <Dragger
          name="files"
          multiple
          accept=".pdf,.doc,.docx,.xls,.xlsx,.jpg,.jpeg,.png"
          beforeUpload={handleBeforeUpload}
          onRemove={handleRemove}
          fileList={fileList}
        >
          <p className="ant-upload-drag-icon">
            <InboxOutlined />
          </p>
          <p className="ant-upload-text">
            Перетащите файлы для загрузки
          </p>
          <p className="ant-upload-hint">
            Поддерживаемые форматы: PDF, DOC, DOCX, XLS, XLSX, JPG, JPEG, PNG
          </p>
        </Dragger>
      </Form.Item>
    </Card>
  );
};

interface WizardStep {
  content: JSX.Element;
}

export const CreateProjectModal = ({ open, onCancel, onSuccess }: CreateProjectModalProps) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [currentStep, setCurrentStep] = useState(0);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [searchLoading, setSearchLoading] = useState(false);
  const [formData, setFormData] = useState<Partial<ProjectFormData>>({});

  useEffect(() => {
    if (open) {
      form.resetFields();
      setCurrentStep(0);
      setFormData({});
    }
  }, [open, form]);

  useEffect(() => {
    if (Object.keys(formData).length > 0) {
      form.setFieldsValue(formData);
    }
  }, [formData, form]);

  const loadEmployees = async (searchText?: string) => {
    try {
      setSearchLoading(true);
      const employeeData = await employeeService.getAllEmployees(searchText);
      setEmployees(Array.isArray(employeeData) ? employeeData : []);
    } catch (error) {
      console.error('Error while loading employees:', error);
      setEmployees([]);
    } finally {
      setSearchLoading(false);
    }
  };

  useEffect(() => {
    if (open && (currentStep === 2 || currentStep === 3)) {
      loadEmployees();
    }
  }, [open, currentStep]);

  const transformFormDataForBackend = (formData: ProjectFormData): CreateProjectRequest => {
  return {
    title: formData.title,
    startDate: formData.startDate.format('YYYY-MM-DD'), 
    endDate: formData.endDate ? formData.endDate.format('YYYY-MM-DD') : undefined,
    priority: formData.priority || 1,
    customerCompanyName: formData.customerCompanyName,
    contractorCompanyName: formData.contractorCompanyName,
    leaderId: formData.leaderId || undefined,
    employeeIds: formData.performerIds || undefined
  };
};

  const steps: WizardStep[] = [
  {
    content: <BasicInfoStep form={form} />
  },
  {
    content: <CompaniesStep form={form} />
  },
  {
    content: <EmployeesSelector 
      employees={employees}
      loading={searchLoading}
      onSearch={loadEmployees}
    />
  },
  {
    content: <LeaderSelector 
      employees={employees}
      loading={searchLoading}
      onSearch={loadEmployees}
    />
  },
  {
    content: <DocumentsStep form={form} />
  }
];
  const handleSubmit = async () => {
  try {
    setLoading(true);
    
    const finalValues = await form.validateFields();
    const allValues = { ...formData, ...finalValues } as ProjectFormData;

    if (!allValues.startDate) {
      message.error('Укажите дату начала');
      return;
    }

    if (allValues.endDate && allValues.endDate.isBefore(allValues.startDate, 'day')) {
      message.error('Дата окончания не может быть раньше даты начала');
      return;
    }

    const projectDataForBackend = transformFormDataForBackend(allValues);
    
    await projectService.createProject(projectDataForBackend);
    
    message.success('Проект успешно создан');
    form.resetFields();
    setCurrentStep(0);
    setFormData({});
    onSuccess();
    
  } catch (error: any) {
    console.error('Error details:', error);
    
    if (error.response?.data) {
      const errorData = error.response.data;
      console.error('Error body:', errorData);
      
      if (errorData.errors && typeof errorData.errors === 'object') {
        const errorMessages = Object.values(errorData.errors).flat().join(', ');
        message.error(`Ошибка валидации: ${errorMessages}`);
      } else if (errorData.title) {
        message.error(`Ошибка: ${errorData.title}`);
      } else if (typeof errorData === 'string') {
        message.error(`Ошибка: ${errorData}`);
      } else {
        message.error('Ошибка создания проекта');
      }
    } else if (error.message) {
      message.error(`Ошибка: ${error.message}`);
    } else {
      message.error('Ошибка создания проекта');
    }
  } finally {
    setLoading(false);
  }
};

  const nextStep = async () => {
    try {
      const fieldNames = getStepFields(currentStep);
      const values = await form.validateFields(fieldNames);
      
      setFormData(prev => ({ ...prev, ...values }));
      setCurrentStep(currentStep + 1);
    } catch (error) {
      console.error("fault validation")
    }
  };

  const prevStep = () => {
    const currentValues = form.getFieldsValue(getStepFields(currentStep));
    setFormData(prev => ({ ...prev, ...currentValues }));
    setCurrentStep(currentStep - 1);
  };

  const getStepFields = (step: number): string[] => {
    const stepFields = [
      ['title', 'startDate', 'endDate', 'priority'],
      ['customerCompanyName', 'contractorCompanyName'],
      ['leaderId'],
      ['performerIds'],
      ['documents']
    ];
    return stepFields[step] || [];
  };

  const handleClose = () => {
    form.resetFields();
    setCurrentStep(0);
    setFormData({});
    onCancel();
  };

  return (
    <Modal
      title="Создание нового проекта"
      open={open}
      onCancel={handleClose}
      footer={null}
      width={800}
      styles={{
        body: { padding: '24px 0', maxHeight: '70vh', overflowY: 'auto' }
      }}
      destroyOnHidden
    >
      <Steps
        current={currentStep}
        items={steps.map((_, index) => ({ title: '' }))}
        style={{ marginBottom: 32, padding: '0 24px' }}
      />

      <Form
        form={form}
        layout="vertical"
        style={{ maxWidth: '100%' }}
        initialValues={{ 
          priority: 1,
          ...formData
        }}
      >
        <div style={{ minHeight: 400, padding: '0 24px' }}>
          {steps[currentStep].content}
        </div>

        <div style={{ marginTop: 32, padding: '0 24px', borderTop: '1px solid #f0f0f0', paddingTop: 16 }}>
          <Flex gap="small" justify="flex-end">
            {currentStep > 0 && (
              <Button onClick={prevStep} size="large">
                Назад
              </Button>
            )}
            <Button onClick={handleClose} size="large">
              Отмена
            </Button>
            {currentStep < steps.length - 1 && (
              <Button type="primary" onClick={nextStep} size="large">
                Далее
              </Button>
            )}
            {currentStep === steps.length - 1 && (
              <Button 
                type="primary" 
                onClick={handleSubmit} 
                loading={loading}
                size="large"
              >
                Создать проект
              </Button>
            )}
          </Flex>
        </div>
      </Form>
    </Modal>
  );
};