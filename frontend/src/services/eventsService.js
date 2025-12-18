import httpClient from './httpClient.js';

// Events API
const eventsService = {
  /**
   * Get list of events with pagination and filters
   * @param {Object} params - Query parameters
   * @param {number} params.page - Page number
   * @param {number} params.pageSize - Items per page
   * @param {string} params.search - Search keyword
   * @param {string} params.status - Filter by status (planned/ongoing/completed)
   * @returns {Promise} Events list response
   */
  getEvents: (params) => {
    return httpClient.get('/events', { params });
  },

  /**
   * Get single event by ID with associated partners
   * @param {number} id - Event ID
   * @returns {Promise} Event details with partners
   */
  getEvent: (id) => {
    return httpClient.get(`/events/${id}`);
  },

  /**
   * Create a new event
   * @param {Object} data - Event data
   * @param {string} data.name - Event name
   * @param {string} data.description - Event description
   * @param {string} data.startDate - Start date (ISO format)
   * @param {string} data.endDate - End date (ISO format)
   * @param {string} data.location - Event location
   * @param {string} data.status - Status (planned/ongoing/completed)
   * @returns {Promise} Created event
   */
  createEvent: (data) => {
    return httpClient.post('/events', data);
  },

  /**
   * Update existing event
   * @param {number} id - Event ID
   * @param {Object} data - Updated event data
   * @returns {Promise} Updated event
   */
  updateEvent: (id, data) => {
    return httpClient.put(`/events/${id}`, data);
  },

  /**
   * Delete event
   * @param {number} id - Event ID
   * @returns {Promise} Delete response
   */
  deleteEvent: (id) => {
    return httpClient.delete(`/events/${id}`);
  },

  /**
   * Add a partner to an event
   * @param {number} eventId - Event ID
   * @param {number} partnerId - Partner ID to add
   * @returns {Promise} Association response
   */
  addPartnerToEvent: (eventId, partnerId) => {
    return httpClient.post(`/events/${eventId}/partners`, { partnerId });
  },

  /**
   * Remove a partner from an event
   * @param {number} eventId - Event ID
   * @param {number} partnerId - Partner ID to remove
   * @returns {Promise} Delete response
   */
  removePartnerFromEvent: (eventId, partnerId) => {
    return httpClient.delete(`/events/${eventId}/partners/${partnerId}`);
  },

  /**
   * Get all partners associated with an event
   * @param {number} eventId - Event ID
   * @returns {Promise} List of partners
   */
  getEventPartners: (eventId) => {
    return httpClient.get(`/events/${eventId}/partners`);
  },
};

export default eventsService;
