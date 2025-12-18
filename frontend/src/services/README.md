# Services Layer

This directory contains all API service modules for the Partner Management System frontend.

## Structure

```
services/
├── index.js              # Central export point
├── api.js                # Main API entry (backward compatibility)
├── httpClient.js         # Base HTTP client with fetch API
├── authService.js        # Authentication API
├── partnersService.js    # Partners management API
├── eventsService.js      # Events management API
└── README.md            # This file
```

## Usage

### Import individual services

```javascript
// Recommended: Import specific service
import authService from '@/services/authService';
import partnersService from '@/services/partnersService';
import eventsService from '@/services/eventsService';

// Use the service
const response = await authService.login({ email, password });
```

### Import from central index

```javascript
// Import multiple services at once
import { authService, partnersService, eventsService } from '@/services';

// Use services
await authService.login(credentials);
await partnersService.getPartners({ page: 1, pageSize: 10 });
```

### Backward compatibility

```javascript
// Old style (still supported)
import { authAPI, partnersAPI, eventsAPI } from '@/services/api';

const response = await authAPI.login(credentials);
```

## HTTP Client

The `httpClient.js` provides a base HTTP client with:

- **Automatic JWT token handling** - Adds Bearer token to all requests
- **Error handling** - Unified error response format
- **401 Auto-logout** - Automatically clears token and redirects on unauthorized
- **Query parameter support** - Easy URL building with params

### Methods

- `httpClient.get(endpoint, { params })` - GET request with query params
- `httpClient.post(endpoint, data)` - POST request with JSON body
- `httpClient.put(endpoint, data)` - PUT request with JSON body
- `httpClient.delete(endpoint)` - DELETE request

## Authentication Service

`authService.js` handles user authentication.

### Methods

```javascript
// Register new user
await authService.register({
  email: 'user@example.com',
  password: 'password123',
  fullName: 'John Doe'
});

// Login
const { data } = await authService.login({
  email: 'user@example.com',
  password: 'password123'
});
// Returns: { token, user: { id, email, userName, fullName } }

// Logout
authService.logout();

// Check authentication
const isAuth = authService.isAuthenticated();

// Get/Set token
const token = authService.getToken();
authService.setToken(newToken);
```

## Partners Service

`partnersService.js` manages partner operations.

### Methods

```javascript
// Get partners list with pagination
const { data } = await partnersService.getPartners({
  page: 1,
  pageSize: 10,
  search: 'keyword',
  status: 'active'
});

// Get single partner
const { data } = await partnersService.getPartner(partnerId);

// Create partner
const { data } = await partnersService.createPartner({
  name: 'Partner Name',
  contactPerson: 'John Doe',
  phone: '123-456-7890',
  email: 'partner@example.com',
  address: '123 Main St',
  description: 'Partner description',
  status: 'active'
});

// Update partner
const { data } = await partnersService.updatePartner(partnerId, updateData);

// Delete partner
await partnersService.deletePartner(partnerId);
```

## Events Service

`eventsService.js` manages event operations and partner associations.

### Methods

```javascript
// Get events list with pagination
const { data } = await eventsService.getEvents({
  page: 1,
  pageSize: 10,
  search: 'keyword',
  status: 'planned'
});

// Get single event with partners
const { data } = await eventsService.getEvent(eventId);

// Create event
const { data } = await eventsService.createEvent({
  name: 'Event Name',
  description: 'Event description',
  startDate: '2024-01-01T10:00:00Z',
  endDate: '2024-01-01T18:00:00Z',
  location: 'Event Location',
  status: 'planned'
});

// Update event
const { data } = await eventsService.updateEvent(eventId, updateData);

// Delete event
await eventsService.deleteEvent(eventId);

// Add partner to event
await eventsService.addPartnerToEvent(eventId, partnerId);

// Remove partner from event
await eventsService.removePartnerFromEvent(eventId, partnerId);

// Get event partners
const { data } = await eventsService.getEventPartners(eventId);
```

## Error Handling

All services return promises that reject with a standardized error format:

```javascript
try {
  const response = await partnersService.getPartners();
} catch (error) {
  console.error('Status:', error.status);
  console.error('Message:', error.message);
  console.error('Errors:', error.errors);
}
```

Error format:
```javascript
{
  status: 400,           // HTTP status code (0 for network errors)
  message: 'Error text', // Error message
  errors: []            // Additional error details
}
```

## Response Format

All successful responses follow this format:

```javascript
{
  data: { /* response data */ },
  status: 200  // HTTP status code
}
```

## Configuration

API base URL is configured in `httpClient.js`:

```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

Change this for production deployment.
