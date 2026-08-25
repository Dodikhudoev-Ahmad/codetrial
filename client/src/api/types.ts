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

export type QuestionType = "SingleChoice" | "MultiChoice" | "ShortAnswer";

export interface AnswerOptionPreviewDto {
  id: string;
  text: string;
}

export interface QuestionPreviewDto {
  id: string;
  order: number;
  type: QuestionType;
  text: string;
  codeSnippet: string | null;
  options: AnswerOptionPreviewDto[];
}

export interface LessonDetailDto {
  id: string;
  courseId: string;
  order: number;
  title: string;
  theoryMarkdown: string;
  xpReward: number;
  questions: QuestionPreviewDto[];
}

export interface AnswerRequest {
  questionId: string;
  givenAnswer: string;
}

export interface SubmitAttemptRequest {
  answers: AnswerRequest[];
}

export interface AnswerOptionResultDto {
  id: string;
  text: string;
  isCorrect: boolean;
}

export interface QuestionResultDto {
  questionId: string;
  questionText: string;
  type: QuestionType;
  codeSnippet: string | null;
  givenAnswer: string;
  isCorrect: boolean;
  explanation: string;
  options: AnswerOptionResultDto[];
  correctShortAnswer: string | null;
}

export interface AttemptResultDto {
  attemptId: string;
  lessonId: string;
  courseSlug: string;
  nextLessonId: string | null;
  scorePercent: number;
  isPassed: boolean;
  attemptNumber: number;
  xpAwarded: number;
  questions: QuestionResultDto[];
}

// Business rule 2: a lesson counts as passed at 70% or higher (mirrors the backend's
// AttemptScoreCalculator.PassingScorePercent).
export const PASSING_SCORE_PERCENT = 70;
