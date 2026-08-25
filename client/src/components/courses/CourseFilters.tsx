import type { CourseLevel } from "../../api/types";

interface CourseFiltersProps {
  level: CourseLevel | "";
  language: string;
  search: string;
  onLevelChange: (level: CourseLevel | "") => void;
  onLanguageChange: (language: string) => void;
  onSearchChange: (search: string) => void;
}

const fieldClasses =
  "rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100";

export function CourseFilters({
  level,
  language,
  search,
  onLevelChange,
  onLanguageChange,
  onSearchChange,
}: CourseFiltersProps) {
  return (
    <div className="mb-8 flex flex-wrap gap-3">
      <input
        type="search"
        placeholder="Поиск по названию или описанию…"
        value={search}
        onChange={(event) => onSearchChange(event.target.value)}
        className={`min-w-[220px] flex-1 ${fieldClasses}`}
        aria-label="Поиск курсов"
      />

      <select
        value={level}
        onChange={(event) => onLevelChange(event.target.value as CourseLevel | "")}
        className={fieldClasses}
        aria-label="Уровень"
      >
        <option value="">Все уровни</option>
        <option value="Beginner">Начальный</option>
        <option value="Intermediate">Средний</option>
        <option value="Advanced">Продвинутый</option>
      </select>

      <input
        type="text"
        placeholder="Язык (например, C#)"
        value={language}
        onChange={(event) => onLanguageChange(event.target.value)}
        className={`w-40 ${fieldClasses}`}
        aria-label="Язык программирования"
      />
    </div>
  );
}
