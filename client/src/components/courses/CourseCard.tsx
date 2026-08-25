import { Link } from "react-router-dom";
import type { CourseSummaryDto } from "../../api/types";
import { pluralizeRu } from "../../utils/pluralizeRu";
import { LevelBadge } from "./LevelBadge";

export function CourseCard({ course }: { course: CourseSummaryDto }) {
  return (
    <Link
      to={`/courses/${course.slug}`}
      className="group flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition-all hover:-translate-y-0.5 hover:border-brand-200 hover:shadow-md"
    >
      <div className="flex items-center justify-between gap-2">
        <LevelBadge level={course.level} />
        <span className="text-xs font-medium text-slate-500">{course.language}</span>
      </div>

      <h2 className="text-lg font-semibold text-slate-900 transition-colors group-hover:text-brand-700">
        {course.title}
      </h2>

      <p className="line-clamp-3 flex-1 text-sm text-slate-600">{course.description}</p>

      <p className="text-xs font-medium text-slate-500">
        {course.lessonsCount} {pluralizeRu(course.lessonsCount, ["урок", "урока", "уроков"])}
      </p>
    </Link>
  );
}
