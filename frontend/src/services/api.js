// Main API entry point - exports all services
import httpClient from './httpClient.js';
import authService from './authService.js';
import partnersService from './partnersService.js';
import eventsService from './eventsService.js';

// Export individual services
export { default as httpClient } from './httpClient.js';
export { default as authService } from './authService.js';
export { default as partnersService } from './partnersService.js';
export { default as eventsService } from './eventsService.js';

// Export with API naming for backward compatibility
export const authAPI = authService;
export const partnersAPI = partnersService;
export const eventsAPI = eventsService;

// Default export
export default httpClient;
