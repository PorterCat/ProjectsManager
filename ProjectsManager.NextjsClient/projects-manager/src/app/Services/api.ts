import axios from 'axios';

const API_BASE_URL = 'https://localhost:7093/api';

export const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  }
});