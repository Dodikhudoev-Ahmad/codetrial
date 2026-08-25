import { httpClient } from "./httpClient";
import type { CourseDetailDto, CourseListParams, CourseSummaryDto, EnrollmentDto, PagedResult } from "./types";

export async function fetchCourses(params: CourseListParams): Promise<PagedResult<CourseSummaryDto>> {
  const { data } = await httpClient.get<PagedResult<CourseSummaryDto>>("/courses", { params });
  return data;
}

export async function fetchCourseBySlug(slug: string): Promise<CourseDetailDto> {
  const { data } = await httpClient.get<CourseDetailDto>(`/courses/${slug}`);
  return data;
}

export async function enrollInCourse(courseId: string): Promise<EnrollmentDto> {
  const { data } = await httpClient.post<EnrollmentDto>(`/courses/${courseId}/enroll`);
  return data;
}
