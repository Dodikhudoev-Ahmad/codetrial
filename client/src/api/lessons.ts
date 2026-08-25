import { httpClient } from "./httpClient";
import type { AttemptResultDto, LessonDetailDto, SubmitAttemptRequest } from "./types";

export async function fetchLesson(lessonId: string): Promise<LessonDetailDto> {
  const { data } = await httpClient.get<LessonDetailDto>(`/lessons/${lessonId}`);
  return data;
}

export async function submitAttempt(
  lessonId: string,
  request: SubmitAttemptRequest,
): Promise<AttemptResultDto> {
  const { data } = await httpClient.post<AttemptResultDto>(`/lessons/${lessonId}/attempts`, request);
  return data;
}
