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
  hasVideo: boolean;
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
  youTubeVideoId: string | null;
  videoWatchedPercent: number;
  questions: QuestionPreviewDto[];
}

export interface VideoProgressDto {
  watchedPercent: number;
}

// Business rule: with a video attached, an attempt can't be submitted until this much
// of it has been watched (mirrors the backend's Lessons.VideoProgressRules).
export const VIDEO_WATCH_THRESHOLD_PERCENT = 60;

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

export interface CourseProgressDto {
  courseId: string;
  courseTitle: string;
  courseSlug: string;
  totalLessons: number;
  passedLessons: number;
  enrolledAt: string;
  completedAt: string | null;
}

export type LeaderboardPeriod = "week" | "all";

export interface LeaderboardEntryDto {
  rank: number;
  userId: string;
  displayName: string;
  xp: number;
}

// --- Admin ---

export interface AdminCourseListItemDto {
  id: string;
  title: string;
  slug: string;
  level: CourseLevel;
  language: string;
  isPublished: boolean;
  lessonsCount: number;
}

export interface AdminLessonSummaryDto {
  id: string;
  order: number;
  title: string;
  xpReward: number;
  questionsCount: number;
}

export interface AdminCourseDetailDto {
  id: string;
  title: string;
  slug: string;
  description: string;
  level: CourseLevel;
  language: string;
  isPublished: boolean;
  lessons: AdminLessonSummaryDto[];
}

export interface UpsertCourseRequest {
  title: string;
  slug: string;
  description: string;
  level: CourseLevel;
  language: string;
  isPublished: boolean;
}

export interface AdminQuestionSummaryDto {
  id: string;
  order: number;
  type: QuestionType;
  text: string;
}

export interface AdminLessonDetailDto {
  id: string;
  courseId: string;
  order: number;
  title: string;
  theoryMarkdown: string;
  xpReward: number;
  youTubeVideoId: string | null;
  questions: AdminQuestionSummaryDto[];
}

export interface UpsertLessonRequest {
  courseId: string;
  order: number;
  title: string;
  theoryMarkdown: string;
  xpReward: number;
  videoUrl: string | null;
}

export interface AdminAnswerOptionDto {
  id: string;
  text: string;
  isCorrect: boolean;
}

export interface AdminQuestionDetailDto {
  id: string;
  lessonId: string;
  order: number;
  type: QuestionType;
  text: string;
  codeSnippet: string | null;
  explanation: string;
  options: AdminAnswerOptionDto[];
  expectedAnswer: string | null;
  isCaseSensitive: boolean;
}

export interface UpsertAnswerOptionRequest {
  text: string;
  isCorrect: boolean;
}

export interface UpsertQuestionRequest {
  lessonId: string;
  type: QuestionType;
  text: string;
  codeSnippet: string | null;
  explanation: string;
  options: UpsertAnswerOptionRequest[];
  expectedAnswer: string | null;
  isCaseSensitive: boolean;
}

export interface LessonStatsDto {
  lessonId: string;
  lessonTitle: string;
  attemptsCount: number;
  studentsPassedCount: number;
  averageScorePercent: number;
}

export interface CourseStatsDto {
  courseId: string;
  courseTitle: string;
  enrollmentsCount: number;
  completionsCount: number;
  averageScorePercent: number;
  lessons: LessonStatsDto[];
}
