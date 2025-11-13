'use client';

import { useTranslation } from 'react-i18next';
import { Dropdown, Space } from 'antd';
import type { MenuProps } from 'antd';
import { GlobalOutlined } from '@ant-design/icons';

const LanguageSwitcher = () => {
  const { i18n } = useTranslation();

  const languages = {
    en: { name: 'English', flag: '🇺🇸' },
    ru: { name: 'Русский', flag: '🇷🇺' }
  };

  const items: MenuProps['items'] = Object.entries(languages).map(([code, lang]) => ({
    key: code,
    label: (
      <Space>
        <span style={{ fontSize: '16px' }}>{lang.flag}</span>
        <span>{lang.name}</span>
      </Space>
    ),
    onClick: () => i18n.changeLanguage(code),
  }));

  const currentLanguage = languages[i18n.language as keyof typeof languages] || languages.en;

  return (
    <Dropdown menu={{ items }} placement="bottomRight">
      <Space style={{ cursor: 'pointer', padding: '8px' }}>
        <GlobalOutlined />
        <span style={{ fontSize: '16px' }}>{currentLanguage.flag}</span>
        <span>{currentLanguage.name}</span>
      </Space>
    </Dropdown>
  );
};

export default LanguageSwitcher;