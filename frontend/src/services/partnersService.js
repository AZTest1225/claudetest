import httpClient from './httpClient.js';

// Partners API
const partnersService = {
  /**
   * Get list of partners with pagination and filters
   * @param {Object} params - Query parameters
   * @param {number} params.page - Page number
   * @param {number} params.pageSize - Items per page
   * @param {string} params.search - Search keyword
   * @param {string} params.status - Filter by status (active/inactive)
   * @returns {Promise} Partners list response
   */
  getPartners: (params) => {
    return httpClient.get('/partners', { params });
  },

  /**
   * Get single partner by ID
   * @param {number} id - Partner ID
   * @returns {Promise} Partner details
   */
  getPartner: (id) => {
    return httpClient.get(`/partners/${id}`);
  },

  /**
   * Create a new partner
   * @param {Object} data - Partner data
   * @param {string} data.name - Partner name
   * @param {string} data.contactPerson - Contact person name
   * @param {string} data.phone - Phone number
   * @param {string} data.email - Email address
   * @param {string} data.address - Address
   * @param {string} data.description - Description
   * @param {string} data.status - Status (active/inactive)
   * @returns {Promise} Created partner
   */
  createPartner: (data) => {
    return httpClient.post('/partners', data);
  },

  /**
   * Update existing partner
   * @param {number} id - Partner ID
   * @param {Object} data - Updated partner data
   * @returns {Promise} Updated partner
   */
  updatePartner: (id, data) => {
    return httpClient.put(`/partners/${id}`, data);
  },

  /**
   * Delete partner
   * @param {number} id - Partner ID
   * @returns {Promise} Delete response
   */
  deletePartner: (id) => {
    return httpClient.delete(`/partners/${id}`);
  },
};

export default partnersService;
