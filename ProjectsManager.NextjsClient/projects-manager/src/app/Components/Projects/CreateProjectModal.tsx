'use client';

import { Modal, Form, Input, DatePicker, Flex, Button, message, Steps, Upload, Select, Card, Tag } from 'antd';
import { CreateProjectRequest } from '@/app/Models/Project';
import { projectService } from "@/app/Services/projectService";
import { employeeService } from "@/app/Services/employeeService";
import dayjs from 'dayjs';
import { useState, useEffect, JSX } from 'react';
import { useTranslation } from 'react-i18next';
import { InboxOutlined, UserOutlined, TeamOutlined } from '@ant-design/icons';
import { Employee } from '@/app/Models/Employee';
import { FormInstance } from 'antd/es/form';
import { TFunction } from 'i18next';
import { RcFile, UploadFile } from 'antd/es/upload';

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
  t: TFunction<'modal'>;
}

interface EmployeeStepProps extends StepProps {
  employees: Employee[];
  loading: boolean;
  onSearch: (searchText?: string) => void;
}

const BasicInfoStep = ({ form, t }: StepProps) => (
  <Flex gap="middle" vertical>
    <Form.Item
      label={t('createProject.projectName')}
      name="title"
      rules={[
        { required: true, message: t('createProject.projectNameRequired') },
        { min: 3, message: t('createProject.projectNameMinError') }
      ]}
    >
      <Input 
        placeholder={t('createProject.projectNamePlaceholder')} 
        size="large"
      />
    </Form.Item>

    <Flex gap="middle">
      <Form.Item
        label={t('createProject.startDate')}
        name="startDate"
        rules={[{ required: true, message: t('createProject.startDateRequired') }]}
        style={{ flex: 1 }}
      >
        <DatePicker 
          format="DD.MM.YYYY"
          placeholder={t('createProject.selectDate')}
          style={{ width: '100%' }}
          size="large"
        />
      </Form.Item>

      <Form.Item
        label={t('createProject.endDate')}
        name="endDate"
        style={{ flex: 1 }}
      >
        <DatePicker 
          format="DD.MM.YYYY"
          placeholder={t('createProject.selectDate')}
          style={{ width: '100%' }}
          size="large"
        />
      </Form.Item>
    </Flex>

    <Form.Item
      label={t('createProject.priority')}
      name="priority"
      rules={[{ required: true, message: t('createProject.priorityRequired') }]}
    >
      <Select size="large" placeholder={t('createProject.selectPriority')}>
        <Option value={1}>{t('createProject.priorityLow')}</Option>
        <Option value={2}>{t('createProject.priorityMedium')}</Option>
        <Option value={3}>{t('createProject.priorityHigh')}</Option>
      </Select>
    </Form.Item>
  </Flex>
);

const CompaniesStep = ({ form, t }: StepProps) => (
  <Flex gap="middle" vertical>
    <Form.Item
      label={t('createProject.customerCompany')}
      name="customerCompanyName"
      rules={[{ required: true, message: t('createProject.customerCompanyRequired') }]}
    >
      <Input 
        placeholder={t('createProject.companyPlaceholder')} 
        size="large"
      />
    </Form.Item>

    <Form.Item
      label={t('createProject.contractorCompany')}
      name="contractorCompanyName"
      rules={[{ required: true, message: t('createProject.contractorCompanyRequired') }]}
    >
      <Input 
        placeholder={t('createProject.companyPlaceholder')} 
        size="large"
      />
    </Form.Item>
  </Flex>
);

const LeaderStep = ({ form, t, employees, loading, onSearch }: EmployeeStepProps) => (
  <Card>
    <Form.Item
      label={t('createProject.selectLeader')}
      name="leaderId"
    >
      <Select
        showSearch
        placeholder={t('createProject.searchLeader')}
        size="large"
        loading={loading}
        onSearch={onSearch}
        filterOption={false}
        optionFilterProp="children"
        suffixIcon={<UserOutlined />}
        allowClear
        notFoundContent={t('createProject.noEmployeesFound')}
        optionLabelProp="label"
        labelInValue={false}
      >
        <Option value={undefined} key="no-leader" label={t('createProject.noLeader')}>
          {t('createProject.noLeader')}
        </Option>
        {Array.isArray(employees) && employees.length > 0 ? (
          employees.map(employee => {
            const fullName = `${employee.firstname} ${employee.lastname}`;
            return (
              <Option 
                key={employee.id.toString()}
                value={employee.id.toString()}
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
          !loading && <Option disabled key="no-data">{t('createProject.noEmployeesFound')}</Option>
        )}
      </Select>
    </Form.Item>
  </Card>
);

const PerformersStep = ({ form, t, employees, loading, onSearch }: EmployeeStepProps) => (
  <Card>
    <Form.Item
      label={t('createProject.selectPerformers')}
      name="performerIds"
    >
      <Select
        mode="multiple"
        showSearch
        placeholder={t('createProject.searchPerformers')}
        size="large"
        loading={loading}
        onSearch={onSearch}
        filterOption={false}
        optionFilterProp="children"
        suffixIcon={<TeamOutlined />}
        notFoundContent={t('createProject.noEmployeesFound')}
        optionLabelProp="label"
        labelInValue={false}
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
          employees.map(employee => {
            const fullName = `${employee.firstname} ${employee.lastname}`;
            return (
              <Option 
                key={employee.id.toString()}
                value={employee.id.toString()}
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
          !loading && <Option disabled key="no-data">{t('createProject.noEmployeesFound')}</Option>
        )}
      </Select>
    </Form.Item>
  </Card>
);

const DocumentsStep = ({ form, t }: StepProps) => {
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
    <Card>
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
            {t('createProject.dragAndDrop')}
          </p>
          <p className="ant-upload-hint">
            {t('createProject.supportedFormats')}
          </p>
        </Dragger>
      </Form.Item>
    </Card>
  );
};

interface WizardStep {
  title: string;
  content: JSX.Element;
}

export const CreateProjectModal = ({ open, onCancel, onSuccess }: CreateProjectModalProps) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [currentStep, setCurrentStep] = useState(0);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [searchLoading, setSearchLoading] = useState(false);
  const [formData, setFormData] = useState<Partial<ProjectFormData>>({});
  const { t } = useTranslation('modal');

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
      const employeeData = await employeeService.searchEmployees(searchText || '');
      setEmployees(Array.isArray(employeeData) ? employeeData : []);
    } catch (error) {
      console.error('Error loading employees:', error);
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

  const steps: WizardStep[] = [
    {
      title: t('createProject.steps.basicInfo'),
      content: <BasicInfoStep form={form} t={t} />
    },
    {
      title: t('createProject.steps.companies'),
      content: <CompaniesStep form={form} t={t} />
    },
    {
      title: t('createProject.steps.leader'),
      content: <LeaderStep 
        form={form} 
        t={t} 
        employees={employees}
        loading={searchLoading}
        onSearch={loadEmployees}
      />
    },
    {
      title: t('createProject.steps.performers'),
      content: <PerformersStep 
        form={form} 
        t={t} 
        employees={employees}
        loading={searchLoading}
        onSearch={loadEmployees}
      />
    },
    {
      title: t('createProject.steps.documents'),
      content: <DocumentsStep form={form} t={t} />
    }
  ];

  const handleSubmit = async () => {
    try {
      setLoading(true);
      
      const finalValues = await form.validateFields();
      const allValues = { ...formData, ...finalValues } as ProjectFormData;
      
      console.log('All form values:', allValues);

      if (!allValues.startDate) {
        message.error(t('createProject.startDateRequired'));
        return;
      }

      const projectDataForBackend = {
        Title: allValues.title,
        StartDate: allValues.startDate.format('YYYY-MM-DD'),
        EndDate: allValues.endDate ? allValues.endDate.format('YYYY-MM-DD') : null,
        Priority: allValues.priority || 1,
        CustomerCompanyName: allValues.customerCompanyName,
        ContractorCompanyName: allValues.contractorCompanyName,
        LeaderId: allValues.leaderId || null
      };
      
      console.log('Sending to backend:', projectDataForBackend);
      
      await projectService.createProject(projectDataForBackend);
      
      message.success(t('createProject.success'));
      form.resetFields();
      setCurrentStep(0);
      setFormData({});
      onSuccess();
      
    } catch (error: any) {
      console.error('Full error details:', error);
      
      if (error.response?.data) {
        const errorData = error.response.data;
        console.error('Error data structure:', errorData);
        
        if (errorData.errors) {
          const errorMessages = Object.values(errorData.errors).flat().join(', ');
          message.error(`${t('createProject.error')}: ${errorMessages}`);
        } else if (errorData.title) {
          message.error(`${t('createProject.error')}: ${errorData.title}`);
        } else if (typeof errorData === 'string') {
          message.error(`${t('createProject.error')}: ${errorData}`);
        } else {
          message.error(t('createProject.error'));
        }
      } else if (error.errorFields) {
        message.warning(t('createProject.validationError'));
      } else {
        message.error(t('createProject.error'));
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
      console.log('Validation failed:', error);
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
      title={t('createProject.title')}
      open={open}
      onCancel={handleClose}
      footer={null}
      width={800}
      styles={{
        body: { padding: '24px 0' }
      }}
    >
      <Steps
        current={currentStep}
        items={steps.map(item => ({ title: item.title }))}
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
                {t('createProject.back')}
              </Button>
            )}
            <Button onClick={handleClose} size="large">
              {t('createProject.cancel')}
            </Button>
            {currentStep < steps.length - 1 && (
              <Button type="primary" onClick={nextStep} size="large">
                {t('createProject.next')}
              </Button>
            )}
            {currentStep === steps.length - 1 && (
              <Button 
                type="primary" 
                onClick={handleSubmit} 
                loading={loading}
                size="large"
              >
                {t('createProject.create')}
              </Button>
            )}
          </Flex>
        </div>
      </Form>
    </Modal>
  );
};