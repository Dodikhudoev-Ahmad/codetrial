import { httpClient } from "./httpClient";
import type { AttemptResultDto, LessonDetailDto, SubmitAttemptRequest, VideoProgressDto } from "./types";

export async function fetchLesson(lessonId: string): Promise<LessonDetailDto> {
  const { data } = await httpClient.get<LessonDetailDto>(`/lessons/${lessonId}`);
  return data;
}

export async function updateVideoProgress(lessonId: string, watchedPercent: number): Promise<VideoProgressDto> {
  const { data } = await httpClient.put<VideoProgressDto>(`/lessons/${lessonId}/video-progress`, { watchedPercent });
  return data;
}

export async function submitAttempt(
  lessonId: string,
  request: SubmitAttemptRequest,
): Promise<AttemptResultDto> {
  const { data } = await httpClient.post<AttemptResultDto>(`/lessons/${lessonId}/attempts`, request);
  return data;
}
