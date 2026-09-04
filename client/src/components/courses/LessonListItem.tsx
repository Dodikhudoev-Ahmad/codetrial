import { Link } from "react-router-dom";
import type { LessonSummaryDto } from "../../api/types";

const STATUS_CONFIG: Record<LessonSummaryDto["status"], { label: string; classes: string; icon: string }> = {
  Passed: { label: "Пройден", classes: "bg-emerald-50 text-emerald-700", icon: "✓" },
  Available: { label: "Доступен", classes: "bg-brand-50 text-brand-700", icon: "▶" },
  Locked: { label: "Заблокирован", classes: "bg-slate-100 text-slate-500", icon: "🔒" },
};

export function LessonListItem({ lesson }: { lesson: LessonSummaryDto }) {
  const config = STATUS_CONFIG[lesson.status];
  const clickable = lesson.status !== "Locked";

  const content = (
    <div
      className={`flex items-center justify-between gap-3 rounded-xl border border-slate-200 bg-white px-4 py-3 ${
        clickable ? "transition-colors hover:border-brand-200 hover:shadow-sm" : "opacity-70"
      }`}
    >
      <div className="flex items-center gap-3">
        <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-slate-100 text-sm font-semibold text-slate-600">
          {lesson.order}
        </span>
        <div>
          <p className="text-sm font-medium text-slate-900">
            {lesson.title}
            {lesson.hasVideo && (
              <span className="ml-1.5 align-middle text-slate-400" aria-label="С видео" title="С видео">
                🎥
              </span>
            )}
          </p>
          <p className="text-xs text-slate-500">{lesson.xpReward} XP</p>
        </div>
      </div>

      <span className={`inline-flex shrink-0 items-center gap-1 rounded-full px-2.5 py-1 text-xs font-medium ${config.classes}`}>
        <span aria-hidden>{config.icon}</span>
        {config.label}
      </span>
    </div>
  );

  if (!clickable) {
    return <div aria-disabled>{content}</div>;
  }

  return (
    <Link to={`/lessons/${lesson.id}`} className="block">
      {content}
    </Link>
  );
}
