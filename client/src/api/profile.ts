import { httpClient } from "./httpClient";
import type { CourseProgressDto } from "./types";

export async function fetchProgress(): Promise<CourseProgressDto[]> {
  const { data } = await httpClient.get<CourseProgressDto[]>("/me/progress");
  return data;
}
