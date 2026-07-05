const API_BASE = '/api';

interface LoginRequest {
  username: string;
  password: string;
}

interface RegisterRequest {
  username: string;
  password: string;
}

interface AuthResponse {
  token: string;
  username: string;
}

interface WeatherData {
  temperature: number;
  place: string;
  condition: string;
  windSpeed: number;
  windDirection: string;
}

// Store token in memory (in a real app, use localStorage with proper security)
let authToken: string | null = null;

export const login = async (username: string, password: string): Promise<AuthResponse | null> => {
  try {
    const response = await fetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password } as LoginRequest),
    });

    if (!response.ok) {
      console.error('Login failed');
      return null;
    }

    const data = await response.json();
    authToken = data.token;
    return data as AuthResponse;
  } catch (error) {
    console.error('Login error:', error);
    return null;
  }
};

export const register = async (username: string, password: string): Promise<AuthResponse | null> => {
  try {
    const response = await fetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password } as RegisterRequest),
    });

    if (!response.ok) {
      console.error('Registration failed');
      return null;
    }

    const data = await response.json();
    authToken = data.token;
    return data as AuthResponse;
  } catch (error) {
    console.error('Registration error:', error);
    return null;
  }
};

export const getCurrentUser = async (): Promise<string | null> => {
  if (!authToken) return null;

  try {
    const response = await fetch(`${API_BASE}/auth/me`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });

    if (!response.ok) {
      console.error('Failed to get current user');
      return null;
    }

    const data = await response.json();
    return data.username;
  } catch (error) {
    console.error('Error getting current user:', error);
    return null;
  }
};

export const getWeather = async (): Promise<WeatherData | null> => {
  if (!authToken) return null;

  try {
    const response = await fetch(`${API_BASE}/weather`, {
      headers: { Authorization: `Bearer ${authToken}` },
    });

    if (!response.ok) {
      console.error('Failed to get weather');
      return null;
    }

    return await response.json() as WeatherData;
  } catch (error) {
    console.error('Error getting weather:', error);
    return null;
  }
};

export const logout = () => {
  authToken = null;
};

export const isAuthenticated = (): boolean => {
  return !!authToken;
};
