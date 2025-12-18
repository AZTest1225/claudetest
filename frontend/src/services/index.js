// Services index - central export point for all API services

export { default as httpClient } from './httpClient.js';
export { default as authService } from './authService.js';
export { default as partnersService } from './partnersService.js';
export { default as eventsService } from './eventsService.js';

// Backward compatibility exports
export { authAPI, partnersAPI, eventsAPI } from './api.js';
