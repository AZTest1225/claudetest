import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authAPI = {
  register: (data) => api.post('/auth/register', data),
  login: (data) => api.post('/auth/login', data),
};

// Partners API
export const partnersAPI = {
  getPartners: (params) => api.get('/partners', { params }),
  getPartner: (id) => api.get(`/partners/${id}`),
  createPartner: (data) => api.post('/partners', data),
  updatePartner: (id, data) => api.put(`/partners/${id}`, data),
  deletePartner: (id) => api.delete(`/partners/${id}`),
};

// Events API
export const eventsAPI = {
  getEvents: (params) => api.get('/events', { params }),
  getEvent: (id) => api.get(`/events/${id}`),
  createEvent: (data) => api.post('/events', data),
  updateEvent: (id, data) => api.put(`/events/${id}`, data),
  deleteEvent: (id) => api.delete(`/events/${id}`),
  addPartnerToEvent: (eventId, partnerId) =>
    api.post(`/events/${eventId}/partners`, { partnerId }),
  removePartnerFromEvent: (eventId, partnerId) =>
    api.delete(`/events/${eventId}/partners/${partnerId}`),
  getEventPartners: (eventId) => api.get(`/events/${eventId}/partners`),
};

export default api;
