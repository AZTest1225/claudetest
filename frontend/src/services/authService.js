import httpClient from './httpClient.js';

// Auth API
const authService = {
  /**
   * Register a new user
   * @param {Object} data - Registration data
   * @param {string} data.email - User email
   * @param {string} data.password - User password
   * @param {string} data.fullName - User full name
   * @returns {Promise} Registration response
   */
  register: (data) => {
    return httpClient.post('/auth/register', data);
  },

  /**
   * Login user
   * @param {Object} data - Login credentials
   * @param {string} data.email - User email
   * @param {string} data.password - User password
   * @returns {Promise} Login response with token and user data
   */
  login: (data) => {
    return httpClient.post('/auth/login', data);
  },

  /**
   * Logout user (clear local token)
   */
  logout: () => {
    localStorage.removeItem('token');
    window.location.href = '/login';
  },

  /**
   * Get current token from localStorage
   * @returns {string|null} JWT token
   */
  getToken: () => {
    return localStorage.getItem('token');
  },

  /**
   * Set token in localStorage
   * @param {string} token - JWT token
   */
  setToken: (token) => {
    localStorage.setItem('token', token);
  },

  /**
   * Check if user is authenticated
   * @returns {boolean} True if token exists
   */
  isAuthenticated: () => {
    return !!localStorage.getItem('token');
  },
};

export default authService;
