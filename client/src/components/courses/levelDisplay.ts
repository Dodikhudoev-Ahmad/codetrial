import type { CourseLevel } from "../../api/types";

export const LEVEL_LABELS: Record<CourseLevel, string> = {
  Beginner: "Начальный",
  Intermediate: "Средний",
  Advanced: "Продвинутый",
};

export const LEVEL_BADGE_CLASSES: Record<CourseLevel, string> = {
  Beginner: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
  Intermediate: "bg-amber-50 text-amber-700 ring-amber-600/20",
  Advanced: "bg-rose-50 text-rose-700 ring-rose-600/20",
};
