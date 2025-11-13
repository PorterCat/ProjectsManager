'use client';

import '@ant-design/v5-patch-for-react-19';
import "./globals.css";
import { AntdRegistry } from '@ant-design/nextjs-registry';
import { Layout, Menu } from 'antd';
import { ProjectOutlined, TeamOutlined } from '@ant-design/icons';
import Link from 'next/link';
import type { MenuProps } from 'antd';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from './Components/LanguageSwitcher';
import './i18n';

const { Header, Sider, Content } = Layout;

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { t } = useTranslation('layout');

  const menuItems: MenuProps['items'] = [
    {
      key: '/projects',
      icon: <ProjectOutlined />,
      label: <Link href="/projects">{t('projects')}</Link>,
    },
    {
      key: '/employees',
      icon: <TeamOutlined />,
      label: <Link href="/employees">{t('employees')}</Link>,
    },
  ];

  return (
    <html lang="en">
      <body>
        <AntdRegistry>
          <Layout style={{ minHeight: '100vh' }}>
            <Sider
              breakpoint="lg"
              collapsedWidth="0"
            >
              <div style={{ 
                height: '32px', 
                margin: '16px', 
                background: 'rgba(255, 255, 255, 0.2)',
                borderRadius: '6px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: 'white',
                fontWeight: 'bold'
              }}>
                {t('menu')}  
              </div>
              <Menu
                theme="dark"
                mode="inline"
                defaultSelectedKeys={['/projects']}
                items={menuItems}
              />
            </Sider>
            <Layout>
              <Header style={{ 
                padding: '0 16px', 
                background: '#fff',
                boxShadow: '0 2px 8px #f0f1f2',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}>
                <h1 style={{ margin: 0, lineHeight: '64px' }}>
                  {t('systemName')}
                </h1>
                <LanguageSwitcher />
              </Header>
              <Content style={{ margin: '24px 16px 0', overflow: 'initial' }}>
                <div style={{ 
                  padding: 24, 
                  background: '#fff',
                  borderRadius: '8px'
                }}>
                  {children}
                </div>
              </Content>
            </Layout>
          </Layout>
        </AntdRegistry>
      </body>
    </html>
  );
}