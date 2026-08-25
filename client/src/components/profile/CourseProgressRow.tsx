import { Link } from "react-router-dom";
import type { CourseProgressDto } from "../../api/types";
import { pluralizeRu } from "../../utils/pluralizeRu";
import { ProgressBar } from "../ProgressBar";

export function CourseProgressRow({ course }: { course: CourseProgressDto }) {
  const percent =
    course.totalLessons === 0 ? 0 : Math.round((course.passedLessons / course.totalLessons) * 100);

  return (
    <Link
      to={`/courses/${course.courseSlug}`}
      className="block rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition-colors hover:border-brand-200"
    >
      <div className="mb-2 flex items-center justify-between gap-3">
        <p className="font-semibold text-slate-900">{course.courseTitle}</p>
        {course.completedAt ? (
          <span className="rounded-full bg-emerald-50 px-2.5 py-0.5 text-xs font-medium text-emerald-700">
            Завершён
          </span>
        ) : (
          <span className="rounded-full bg-brand-50 px-2.5 py-0.5 text-xs font-medium text-brand-700">
            В процессе
          </span>
        )}
      </div>
      <ProgressBar value={percent} />
      <p className="mt-1.5 text-xs text-slate-500">
        {course.passedLessons} из {course.totalLessons}{" "}
        {pluralizeRu(course.totalLessons, ["урок", "урока", "уроков"])}
      </p>
    </Link>
  );
}
