import { useState } from "react";
import { fetchLeaderboard } from "../api/leaderboard";
import type { LeaderboardPeriod } from "../api/types";
import { useAuth } from "../auth/useAuth";
import { EmptyState } from "../components/EmptyState";
import { ErrorState } from "../components/ErrorState";
import { LeaderboardTable } from "../components/leaderboard/LeaderboardTable";
import { LoadingState } from "../components/LoadingState";
import { PageHeader } from "../components/PageHeader";
import { useAsyncData } from "../hooks/useAsyncData";

const TABS: { value: LeaderboardPeriod; label: string }[] = [
  { value: "week", label: "За неделю" },
  { value: "all", label: "За всё время" },
];

export function LeaderboardPage() {
  const { user } = useAuth();
  const [period, setPeriod] = useState<LeaderboardPeriod>("all");

  const leaderboard = useAsyncData(() => fetchLeaderboard(period), [period]);

  return (
    <div>
      <PageHeader title="Рейтинг" description="Топ студентов по опыту." />

      <div className="mb-6 inline-flex rounded-lg border border-slate-200 bg-white p-1">
        {TABS.map((tab) => (
          <button
            key={tab.value}
            type="button"
            onClick={() => setPeriod(tab.value)}
            className={`rounded-md px-4 py-1.5 text-sm font-medium transition-colors ${
              period === tab.value ? "bg-brand-600 text-white" : "text-slate-600 hover:bg-slate-50"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {leaderboard.status === "loading" && <LoadingState label="Загружаем рейтинг…" />}

      {leaderboard.status === "error" && <ErrorState onRetry={leaderboard.reload} />}

      {leaderboard.status === "success" && leaderboard.data.length === 0 && (
        <EmptyState title="Пока никто не набрал опыт" description="Станьте первым в рейтинге!" />
      )}

      {leaderboard.status === "success" && leaderboard.data.length > 0 && (
        <LeaderboardTable entries={leaderboard.data} currentUserId={user?.id} />
      )}
    </div>
  );
}
