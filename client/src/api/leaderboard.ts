import { httpClient } from "./httpClient";
import type { LeaderboardEntryDto, LeaderboardPeriod } from "./types";

export async function fetchLeaderboard(period: LeaderboardPeriod): Promise<LeaderboardEntryDto[]> {
  const { data } = await httpClient.get<LeaderboardEntryDto[]>("/leaderboard", { params: { period } });
  return data;
}
