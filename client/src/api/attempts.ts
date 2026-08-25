import { httpClient } from "./httpClient";
import type { AttemptResultDto } from "./types";

export async function fetchAttempt(attemptId: string): Promise<AttemptResultDto> {
  const { data } = await httpClient.get<AttemptResultDto>(`/attempts/${attemptId}`);
  return data;
}
