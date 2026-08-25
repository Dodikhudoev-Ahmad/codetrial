import { httpClient } from "./httpClient";
import type {
  AdminCourseDetailDto,
  AdminCourseListItemDto,
  AdminLessonDetailDto,
  AdminQuestionDetailDto,
  CourseStatsDto,
  PagedResult,
  UpsertCourseRequest,
  UpsertLessonRequest,
  UpsertQuestionRequest,
} from "./types";

export async function fetchAdminCourses(page = 1, pageSize = 50): Promise<PagedResult<AdminCourseListItemDto>> {
  const { data } = await httpClient.get<PagedResult<AdminCourseListItemDto>>("/admin/courses", {
    params: { page, pageSize },
  });
  return data;
}

export async function fetchAdminCourse(id: string): Promise<AdminCourseDetailDto> {
  const { data } = await httpClient.get<AdminCourseDetailDto>(`/admin/courses/${id}`);
  return data;
}

export async function createAdminCourse(request: UpsertCourseRequest): Promise<AdminCourseDetailDto> {
  const { data } = await httpClient.post<AdminCourseDetailDto>("/admin/courses", request);
  return data;
}

export async function updateAdminCourse(id: string, request: UpsertCourseRequest): Promise<AdminCourseDetailDto> {
  const { data } = await httpClient.put<AdminCourseDetailDto>(`/admin/courses/${id}`, request);
  return data;
}

export async function deleteAdminCourse(id: string): Promise<void> {
  await httpClient.delete(`/admin/courses/${id}`);
}

export async function fetchAdminCourseStats(id: string): Promise<CourseStatsDto> {
  const { data } = await httpClient.get<CourseStatsDto>(`/admin/courses/${id}/stats`);
  return data;
}

export async function fetchAdminLesson(id: string): Promise<AdminLessonDetailDto> {
  const { data } = await httpClient.get<AdminLessonDetailDto>(`/admin/lessons/${id}`);
  return data;
}

export async function createAdminLesson(request: UpsertLessonRequest): Promise<AdminLessonDetailDto> {
  const { data } = await httpClient.post<AdminLessonDetailDto>("/admin/lessons", request);
  return data;
}

export async function updateAdminLesson(id: string, request: UpsertLessonRequest): Promise<AdminLessonDetailDto> {
  const { data } = await httpClient.put<AdminLessonDetailDto>(`/admin/lessons/${id}`, request);
  return data;
}

export async function deleteAdminLesson(id: string): Promise<void> {
  await httpClient.delete(`/admin/lessons/${id}`);
}

export async function fetchAdminQuestion(id: string): Promise<AdminQuestionDetailDto> {
  const { data } = await httpClient.get<AdminQuestionDetailDto>(`/admin/questions/${id}`);
  return data;
}

export async function createAdminQuestion(request: UpsertQuestionRequest): Promise<AdminQuestionDetailDto> {
  const { data } = await httpClient.post<AdminQuestionDetailDto>("/admin/questions", request);
  return data;
}

export async function updateAdminQuestion(id: string, request: UpsertQuestionRequest): Promise<AdminQuestionDetailDto> {
  const { data } = await httpClient.put<AdminQuestionDetailDto>(`/admin/questions/${id}`, request);
  return data;
}

export async function deleteAdminQuestion(id: string): Promise<void> {
  await httpClient.delete(`/admin/questions/${id}`);
}
