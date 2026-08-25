import { createContext, useCallback, useEffect, useMemo, useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import { getStoredToken, httpClient, registerUnauthorizedHandler, setStoredToken } from "../api/httpClient";
import type { AuthResponse, LoginRequest, RegisterRequest, UserDto } from "../api/types";

interface AuthContextValue {
  user: UserDto | null;
  isLoading: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
}

// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(() => Boolean(getStoredToken()));
  const navigate = useNavigate();

  const logout = useCallback(() => {
    setStoredToken(null);
    setUser(null);
    navigate("/login");
  }, [navigate]);

  // Any 401 response, from any request, should log the user out - registered once here
  // rather than handled ad hoc wherever a request happens to fail.
  useEffect(() => {
    registerUnauthorizedHandler(logout);
  }, [logout]);

  // Restore the session on first load: if a token is already stored, ask the API who
  // it belongs to instead of trusting stale localStorage state.
  useEffect(() => {
    const token = getStoredToken();
    if (!token) {
      return;
    }

    httpClient
      .get<UserDto>("/auth/me")
      .then((response) => setUser(response.data))
      .catch(() => setStoredToken(null))
      .finally(() => setIsLoading(false));
  }, []);

  const login = useCallback(async (request: LoginRequest) => {
    const response = await httpClient.post<AuthResponse>("/auth/login", request);
    setStoredToken(response.data.token);
    setUser(response.data.user);
  }, []);

  const register = useCallback(async (request: RegisterRequest) => {
    const response = await httpClient.post<AuthResponse>("/auth/register", request);
    setStoredToken(response.data.token);
    setUser(response.data.user);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({ user, isLoading, login, register, logout }),
    [user, isLoading, login, register, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
