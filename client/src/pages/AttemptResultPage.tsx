import { Link, useNavigate, useParams } from "react-router-dom";
import { fetchAttempt } from "../api/attempts";
import { QuestionResultCard } from "../components/attempts/QuestionResultCard";
import { ScoreSummary } from "../components/attempts/ScoreSummary";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { useAsyncData } from "../hooks/useAsyncData";

export function AttemptResultPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const attempt = useAsyncData(() => fetchAttempt(id!), [id]);

  if (attempt.status === "loading") {
    return <LoadingState label="Загружаем результат…" />;
  }

  if (attempt.status === "error") {
    return <ErrorState onRetry={attempt.reload} />;
  }

  const { data } = attempt;

  return (
    <div>
      <h1 className="mb-6 text-3xl font-extrabold tracking-tight text-slate-900">Результат попытки</h1>

      <ScoreSummary result={data} />

      <div className="space-y-4">
        {data.questions.map((question, index) => (
          <QuestionResultCard key={question.questionId} result={question} index={index + 1} />
        ))}
      </div>

      <div className="mt-8 flex flex-wrap items-center gap-3">
        <button
          type="button"
          onClick={() => navigate(`/lessons/${data.lessonId}`)}
          className="rounded-lg border border-slate-200 bg-white px-5 py-2.5 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-50"
        >
          Пройти ещё раз
        </button>

        {data.isPassed && data.nextLessonId && (
          <button
            type="button"
            onClick={() => navigate(`/lessons/${data.nextLessonId}`)}
            className="rounded-lg bg-brand-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700"
          >
            Следующий урок →
          </button>
        )}

        {data.isPassed && !data.nextLessonId && (
          <Link
            to={`/courses/${data.courseSlug}`}
            className="rounded-lg bg-brand-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700"
          >
            Курс завершён — к странице курса
          </Link>
        )}
      </div>
    </div>
  );
}
