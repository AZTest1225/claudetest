const API_BASE_URL = 'http://localhost:5000/api';

// Helper function to build URL with query parameters
const buildUrl = (endpoint, params) => {
  const url = new URL(`${API_BASE_URL}${endpoint}`);
  if (params) {
    Object.keys(params).forEach(key => {
      if (params[key] !== null && params[key] !== undefined) {
        url.searchParams.append(key, params[key]);
      }
    });
  }
  return url.toString();
};

// Helper function to handle fetch requests
const fetchWithAuth = async (url, options = {}) => {
  const token = localStorage.getItem('token');

  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const config = {
    ...options,
    headers,
  };

  try {
    const response = await fetch(url, config);

    // Handle 401 Unauthorized
    if (response.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
      throw new Error('Unauthorized');
    }

    // Parse JSON response
    const data = await response.json().catch(() => null);

    if (!response.ok) {
      throw {
        status: response.status,
        message: data?.message || response.statusText,
        errors: data?.errors || [],
      };
    }

    return { data, status: response.status };
  } catch (error) {
    if (error.status) {
      throw error;
    }
    throw {
      status: 0,
      message: error.message || 'Network error',
      errors: [],
    };
  }
};

// HTTP methods
const api = {
  get: (endpoint, options = {}) => {
    const url = buildUrl(endpoint, options.params);
    return fetchWithAuth(url, {
      method: 'GET',
    });
  },

  post: (endpoint, data, options = {}) => {
    const url = `${API_BASE_URL}${endpoint}`;
    return fetchWithAuth(url, {
      method: 'POST',
      body: JSON.stringify(data),
      ...options,
    });
  },

  put: (endpoint, data, options = {}) => {
    const url = `${API_BASE_URL}${endpoint}`;
    return fetchWithAuth(url, {
      method: 'PUT',
      body: JSON.stringify(data),
      ...options,
    });
  },

  delete: (endpoint, options = {}) => {
    const url = `${API_BASE_URL}${endpoint}`;
    return fetchWithAuth(url, {
      method: 'DELETE',
      ...options,
    });
  },
};

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
