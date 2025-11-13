import i18next from "i18next";
import { initReactI18next } from "react-i18next";

import enCommon from '../locales/en/common.json';
import enLayout from '../locales/en/layout.json';
import enProjects from '../locales/en/projects.json';
import enModal from '../locales/en/modal.json';

import ruCommon from '../locales/ru/common.json';
import ruLayout from '../locales/ru/layout.json';
import ruProjects from '../locales/ru/projects.json';
import ruModal from '../locales/ru/modal.json';

const resources = {
  en: {
    common: enCommon,
    layout: enLayout,
    projects: enProjects,
    modal: enModal
  },
  ru: {
    common: ruCommon,
    layout: ruLayout,
    projects: ruProjects,
    modal: ruModal
  }
};

i18next
  .use(initReactI18next)
  .init({
    resources,
    lng: 'ru', 
    fallbackLng: 'en',
    debug: process.env.NODE_ENV === 'development',
    defaultNS: 'common',
    interpolation: {
      escapeValue: false,
    }
  });

export default i18next;