import axios from 'axios';
import { AdminSettings, ProblemBalloonMap, LoginRequest, BalloonRequestDTO, BalloonStatusUpdateRequest, AdminSettingsResponse } from '../types';

const api = axios.create({
  baseURL: '/api',
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const login = async (credentials: LoginRequest) => {
  const response = await api.post('/admin/login', credentials);
  return response.data;
};

export const getAdminSettings = async () => {
  const response = await api.get('/admin/settings');
  return response.data as AdminSettings;
};

export const updateAdminSettings = async (settings: Partial<AdminSettings>) => {
  const response = await api.post('/admin/settings/update', settings);
  return response.data as AdminSettings;
};

export const getProblemBalloonMaps = async () => {
  const response = await api.get('/admin/settings/ProblemBalloonMap/getAll');
  return response.data as ProblemBalloonMap[];
};

export const createProblemBalloonMap = async (map: Omit<ProblemBalloonMap, 'id'> & { adminSettingsId: string }) => {
  const response = await api.post('/admin/settings/ProblemBalloonMap/create', map);
  return response.data as ProblemBalloonMap;
};

export const updateProblemBalloonMap = async (map: ProblemBalloonMap) => {
  const response = await api.post('/admin/settings/ProblemBalloonMap/update', map);
  return response.data as ProblemBalloonMap;
};

export const deleteProblemBalloonMap = async (id: string) => {
  await api.post('/admin/settings/ProblemBalloonMap/delete', null, {
    params: { id }
  });
};

export const getPendingBalloons = async () => {
  const response = await api.get('/balloon/pending');
  return Array.isArray(response.data.$values) ? response.data.$values : response.data;
};

export const getPickedUpBalloons = async () => {
  const response = await api.get('/balloon/picked-up');
  return Array.isArray(response.data.$values) ? response.data.$values : response.data;
};

export const getDeliveredBalloons = async () => {
  const response = await api.get('/balloon/delivered');
  return Array.isArray(response.data.$values) ? response.data.$values : response.data;
};

export const updateBalloonStatus = async (id: string, request: BalloonStatusUpdateRequest) => {
  const response = await api.put('/balloon/status', {
    id,
    status: request.status === 'Pending' ? 0 : request.status === 'PickedUp' ? 1 : 2,
    deliveredBy: request.deliveredBy
  });
  return response.data as BalloonRequestDTO;
};

export const getAllAdminSettings = async () => {
  const response = await api.get('/admin/settings/getAll');
  return response.data as AdminSettingsResponse;
};

export const getActiveAdminSettings = async () => {
  const response = await api.get('/admin/settings/getActive');
  return response.data as AdminSettings;
};

export const createAdminSettings = async (settings: Omit<AdminSettings, 'id'>) => {
  const response = await api.post('/admin/settings/create', settings);
  return response.data as AdminSettings;
};

export const setActiveAdminSettings = async (id: string) => {
  await api.post('/admin/settings/enable', null, {
    params: { id }
  });
}; 