import axios from "axios";
import { getToken, removeToken } from "./auth";

const API_BASE_URL = process.env.NEXT_BACKEND_URL;

export const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle token expiration
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      removeToken();
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);

export const authAPI = {
  login: (credentials: { email: string; password: string }) =>
    api.post("/auth/login", credentials),
  register: (credentials: { email: string; password: string }) =>
    api.post("/auth/register", credentials),
};

export const intrusionAPI = {
  getIntrusions: (page: number = 1, pageSize: number = 20) =>
    api.get(`/intrusion?page=${page}&pageSize=${pageSize}`),
  getIntrusion: (id: number) => api.get(`/intrusion/${id}`),
  createIntrusion: (data: any) => api.post("/intrusion", data),
  resolveIntrusion: (id: number) => api.patch(`/intrusion/${id}/resolve`),
  getRecentIntrusions: () => api.get("/intrusion/recent"),
  getDashboardStats: () => api.get("/intrusion/dashboard/stats"),
};

export const alertAPI = {
  getAlerts: (unacknowledgedOnly: boolean = false) =>
    api.get(`/alert?unacknowledgedOnly=${unacknowledgedOnly}`),
  acknowledgeAlert: (id: number) => api.patch(`/alert/${id}/acknowledge`),
};
