import type { CourseLevel } from "../../api/types";
import { LEVEL_BADGE_CLASSES, LEVEL_LABELS } from "./levelDisplay";

export function LevelBadge({ level }: { level: CourseLevel }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset ${LEVEL_BADGE_CLASSES[level]}`}
    >
      {LEVEL_LABELS[level]}
    </span>
  );
}
