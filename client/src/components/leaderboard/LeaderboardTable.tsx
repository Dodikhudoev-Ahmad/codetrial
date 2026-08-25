import type { LeaderboardEntryDto } from "../../api/types";

const MEDALS: Record<number, string> = { 1: "🥇", 2: "🥈", 3: "🥉" };

export function LeaderboardTable({ entries, currentUserId }: { entries: LeaderboardEntryDto[]; currentUserId?: string }) {
  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      {entries.map((entry) => (
        <div
          key={entry.userId}
          className={`flex items-center gap-4 border-b border-slate-100 px-5 py-3 last:border-b-0 ${
            entry.userId === currentUserId ? "bg-brand-50" : ""
          }`}
        >
          <span className="w-8 text-center text-sm font-semibold text-slate-500">
            {MEDALS[entry.rank] ?? entry.rank}
          </span>
          <span className="flex-1 font-medium text-slate-900">{entry.displayName}</span>
          <span className="text-sm font-semibold text-brand-600">{entry.xp} XP</span>
        </div>
      ))}
    </div>
  );
}
