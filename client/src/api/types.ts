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

export type CourseLevel = "Beginner" | "Intermediate" | "Advanced";

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CourseSummaryDto {
  id: string;
  title: string;
  slug: string;
  description: string;
  level: CourseLevel;
  language: string;
  lessonsCount: number;
}

export type LessonStatus = "Locked" | "Available" | "Passed";

export interface LessonSummaryDto {
  id: string;
  order: number;
  title: string;
  xpReward: number;
  status: LessonStatus;
}

export interface CourseDetailDto {
  id: string;
  title: string;
  slug: string;
  description: string;
  level: CourseLevel;
  language: string;
  isEnrolled: boolean;
  lessons: LessonSummaryDto[];
}

export interface EnrollmentDto {
  courseId: string;
  enrolledAt: string;
}

export interface CourseListParams {
  level?: CourseLevel;
  language?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}
