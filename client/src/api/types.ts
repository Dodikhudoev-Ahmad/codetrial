// Mirrors the DTOs returned by CodeTrail.Api (CodeTrail.Application.Auth.Dtos et al.)
// Keep these in sync with the backend contract by hand for now - no shared codegen yet.

export type UserRole = "Student" | "Admin";

export interface UserDto {
  id: string;
  email: string;
  displayName: string;
  role: UserRole;
  totalXp: number;
  currentStreak: number;
  lastActivityDate: string | null;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserDto;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
}
