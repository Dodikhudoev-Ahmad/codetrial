import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { extractErrorMessage } from "../api/errors";
import { fetchLesson, submitAttempt, updateVideoProgress } from "../api/lessons";
import { VIDEO_WATCH_THRESHOLD_PERCENT } from "../api/types";
import { ErrorBanner } from "../components/form/ErrorBanner";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { LessonVideo } from "../components/lessons/LessonVideo";
import { MultiChoiceQuestion } from "../components/lessons/MultiChoiceQuestion";
import { QuestionCard } from "../components/lessons/QuestionCard";
import { ShortAnswerQuestion } from "../components/lessons/ShortAnswerQuestion";
import { SingleChoiceQuestion } from "../components/lessons/SingleChoiceQuestion";
import { TheoryRenderer } from "../components/lessons/TheoryRenderer";
import { ProgressBar } from "../components/ProgressBar";
import { useAsyncData } from "../hooks/useAsyncData";

export function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const lesson = useAsyncData(() => fetchLesson(id!), [id]);
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [watchedPercent, setWatchedPercent] = useState(0);
  const reportedPercentRef = useRef(0);

  useEffect(() => {
    if (lesson.status === "success") {
      setWatchedPercent(lesson.data.videoWatchedPercent);
      reportedPercentRef.current = lesson.data.videoWatchedPercent;
    }
    // Re-sync whenever a fetch for this (or a new) lesson resolves, but not on every
    // re-render once loaded - local progress updates shouldn't be clobbered back down.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id, lesson.status]);

  const handleVideoProgress = (percent: number) => {
    if (lesson.status !== "success") return;
    setWatchedPercent((prev) => Math.max(prev, percent));

    // Only worth telling the server about forward progress, and only once per poll tick.
    if (percent > reportedPercentRef.current) {
      reportedPercentRef.current = percent;
      updateVideoProgress(lesson.data.id, percent).catch(() => {
        // Best-effort - a dropped progress ping just means the next one carries it instead.
      });
    }
  };

  const setAnswer = (questionId: string, value: string) => {
    setAnswers((prev) => ({ ...prev, [questionId]: value }));
  };

  const handleSubmit = async () => {
    if (lesson.status !== "success") return;
    setSubmitError(null);
    setIsSubmitting(true);

    try {
      const result = await submitAttempt(lesson.data.id, {
        answers: lesson.data.questions.map((question) => ({
          questionId: question.id,
          givenAnswer: answers[question.id] ?? "",
        })),
      });
      navigate(`/attempts/${result.attemptId}`);
    } catch (error) {
      setSubmitError(
        extractErrorMessage(error, {
          403: "Досмотрите видео или подождите до завтра — превышен дневной лимит попыток (максимум 5).",
          400: "Не удалось отправить ответы. Обновите страницу и попробуйте снова.",
        }),
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  if (lesson.status === "loading") {
    return <LoadingState label="Загружаем урок…" />;
  }

  if (lesson.status === "error") {
    return <ErrorState onRetry={lesson.reload} />;
  }

  const { data } = lesson;
  const allAnswered = data.questions.every((question) => (answers[question.id] ?? "").trim().length > 0);
  const videoRequired = Boolean(data.youTubeVideoId);
  const videoRequirementMet = !videoRequired || watchedPercent >= VIDEO_WATCH_THRESHOLD_PERCENT;

  return (
    <div>
      <p className="mb-6 text-sm font-medium text-brand-600">
        Урок {data.order} · {data.title}
      </p>

      {data.youTubeVideoId && (
        <LessonVideo videoId={data.youTubeVideoId} title={data.title} onProgress={handleVideoProgress} />
      )}

      <TheoryRenderer markdown={data.theoryMarkdown} />

      {data.questions.length > 0 && (
        <div className="mt-10">
          <h2 className="mb-4 text-xl font-bold text-slate-900">Проверьте себя</h2>

          {!videoRequirementMet ? (
            <div className="rounded-xl border border-dashed border-slate-300 bg-slate-50 px-6 py-8 text-center">
              <p className="mb-1 font-medium text-slate-700">🔒 Тест откроется после просмотра видео</p>
              <p className="mb-4 text-sm text-slate-500">
                Досмотрите не менее {VIDEO_WATCH_THRESHOLD_PERCENT}% ролика выше, чтобы проверить себя.
              </p>
              <div className="mx-auto max-w-xs">
                <ProgressBar value={(watchedPercent / VIDEO_WATCH_THRESHOLD_PERCENT) * 100} />
                <p className="mt-2 text-xs text-slate-400">
                  Просмотрено {watchedPercent}% из {VIDEO_WATCH_THRESHOLD_PERCENT}%
                </p>
              </div>
            </div>
          ) : (
            <>
              <div className="space-y-4">
                {data.questions.map((question, index) => (
                  <QuestionCard key={question.id} index={index + 1} question={question}>
                    {question.type === "SingleChoice" && (
                      <SingleChoiceQuestion
                        question={question}
                        value={answers[question.id] ?? ""}
                        onChange={(value) => setAnswer(question.id, value)}
                      />
                    )}
                    {question.type === "MultiChoice" && (
                      <MultiChoiceQuestion
                        question={question}
                        value={answers[question.id] ?? ""}
                        onChange={(value) => setAnswer(question.id, value)}
                      />
                    )}
                    {question.type === "ShortAnswer" && (
                      <ShortAnswerQuestion
                        value={answers[question.id] ?? ""}
                        onChange={(value) => setAnswer(question.id, value)}
                      />
                    )}
                  </QuestionCard>
                ))}
              </div>

              <ErrorBanner message={submitError} />

              <button
                type="button"
                onClick={handleSubmit}
                disabled={!allAnswered || isSubmitting}
                className="mt-6 w-full rounded-lg bg-brand-600 px-4 py-3 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto"
              >
                {isSubmitting ? "Проверяем…" : "Проверить"}
              </button>
            </>
          )}
        </div>
      )}
    </div>
  );
}
