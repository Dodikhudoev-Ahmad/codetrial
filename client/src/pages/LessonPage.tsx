import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { extractErrorMessage } from "../api/errors";
import { fetchLesson, submitAttempt } from "../api/lessons";
import { ErrorBanner } from "../components/form/ErrorBanner";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { MultiChoiceQuestion } from "../components/lessons/MultiChoiceQuestion";
import { QuestionCard } from "../components/lessons/QuestionCard";
import { ShortAnswerQuestion } from "../components/lessons/ShortAnswerQuestion";
import { SingleChoiceQuestion } from "../components/lessons/SingleChoiceQuestion";
import { TheoryRenderer } from "../components/lessons/TheoryRenderer";
import { useAsyncData } from "../hooks/useAsyncData";

export function LessonPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const lesson = useAsyncData(() => fetchLesson(id!), [id]);
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

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
          403: "Превышен лимит попыток на сегодня (максимум 5). Попробуйте завтра.",
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

  return (
    <div>
      <p className="mb-6 text-sm font-medium text-brand-600">
        Урок {data.order} · {data.title}
      </p>

      <TheoryRenderer markdown={data.theoryMarkdown} />

      {data.questions.length > 0 && (
        <div className="mt-10">
          <h2 className="mb-4 text-xl font-bold text-slate-900">Проверьте себя</h2>

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
        </div>
      )}
    </div>
  );
}
