import axios from "axios";

// The one and only axios instance for the app - every request goes through this
// (no scattered fetch/axios calls in components), so the JWT header and 401 handling
// below apply everywhere automatically.
const baseURL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5094/api";

export const httpClient = axios.create({ baseURL });

const TOKEN_STORAGE_KEY = "codetrail.token";

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setStoredToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_STORAGE_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
  }
}

httpClient.interceptors.request.use((config) => {
  const token = getStoredToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// AuthContext registers itself here so a 401 anywhere logs the user out and redirects,
// without httpClient needing to import React/router code directly.
type UnauthorizedHandler = () => void;
let onUnauthorized: UnauthorizedHandler | null = null;

export function registerUnauthorizedHandler(handler: UnauthorizedHandler): void {
  onUnauthorized = handler;
}

httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      onUnauthorized?.();
    }
    return Promise.reject(error);
  },
);
