import { useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { enrollInCourse, fetchCourseBySlug } from "../api/courses";
import { extractErrorMessage } from "../api/errors";
import { useAuth } from "../auth/useAuth";
import { LessonListItem } from "../components/courses/LessonListItem";
import { LevelBadge } from "../components/courses/LevelBadge";
import { ErrorBanner } from "../components/form/ErrorBanner";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { ProgressBar } from "../components/ProgressBar";
import { useAsyncData } from "../hooks/useAsyncData";
import { pluralizeRu } from "../utils/pluralizeRu";

export function CourseDetailPage() {
  const { slug } = useParams<{ slug: string }>();
  const { user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [enrollError, setEnrollError] = useState<string | null>(null);
  const [isEnrolling, setIsEnrolling] = useState(false);

  const course = useAsyncData(() => fetchCourseBySlug(slug!), [slug]);

  const handleEnroll = async () => {
    if (course.status !== "success") return;
    setEnrollError(null);
    setIsEnrolling(true);

    try {
      await enrollInCourse(course.data.id);
      course.reload();
    } catch (error) {
      setEnrollError(extractErrorMessage(error, { 409: "Вы уже записаны на этот курс." }));
    } finally {
      setIsEnrolling(false);
    }
  };

  if (course.status === "loading") {
    return <LoadingState label="Загружаем курс…" />;
  }

  if (course.status === "error") {
    return <ErrorState onRetry={course.reload} />;
  }

  const { data } = course;
  const passedCount = data.lessons.filter((lesson) => lesson.status === "Passed").length;
  const progress = data.lessons.length === 0 ? 0 : Math.round((passedCount / data.lessons.length) * 100);

  return (
    <div>
      <div className="mb-8 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="mb-2 flex items-center gap-2">
            <LevelBadge level={data.level} />
            <span className="text-sm text-slate-500">{data.language}</span>
          </div>
          <h1 className="text-3xl font-extrabold tracking-tight text-slate-900">{data.title}</h1>
          <p className="mt-2 max-w-2xl text-slate-600">{data.description}</p>
        </div>

        {user?.role === "Student" && !data.isEnrolled && (
          <button
            type="button"
            onClick={handleEnroll}
            disabled={isEnrolling}
            className="shrink-0 rounded-lg bg-brand-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isEnrolling ? "Записываем…" : "Записаться"}
          </button>
        )}

        {!user && (
          <button
            type="button"
            onClick={() => navigate("/login", { state: { from: location } })}
            className="shrink-0 rounded-lg border border-slate-200 bg-white px-5 py-2.5 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-50"
          >
            Войдите, чтобы записаться
          </button>
        )}
      </div>

      <ErrorBanner message={enrollError} />

      {data.isEnrolled && data.lessons.length > 0 && (
        <div className="mb-6">
          <div className="mb-1 flex items-center justify-between text-sm text-slate-600">
            <span>Прогресс</span>
            <span>
              {passedCount} из {data.lessons.length} {pluralizeRu(data.lessons.length, ["урок", "урока", "уроков"])}
            </span>
          </div>
          <ProgressBar value={progress} />
        </div>
      )}

      <div className="space-y-2">
        {data.lessons.map((lesson) => (
          <LessonListItem key={lesson.id} lesson={lesson} />
        ))}
      </div>
    </div>
  );
}
