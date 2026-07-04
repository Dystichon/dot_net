export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
}

export interface UserInfo {
  username: string;
}

export interface WeatherData {
  temperature: number;
  place: string;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
}
