import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  createAdminQuestion,
  deleteAdminQuestion,
  fetchAdminLesson,
  fetchAdminQuestion,
  updateAdminLesson,
  updateAdminQuestion,
} from "../../api/admin";
import { extractErrorMessage } from "../../api/errors";
import type { AdminQuestionDetailDto } from "../../api/types";
import { LessonForm } from "../../components/admin/LessonForm";
import { QuestionForm } from "../../components/admin/QuestionForm";
import { ConfirmDialog } from "../../components/ConfirmDialog";
import { ErrorState } from "../../components/ErrorState";
import { LoadingState } from "../../components/LoadingState";
import { Modal } from "../../components/Modal";
import { PageHeader } from "../../components/PageHeader";
import { useAsyncData } from "../../hooks/useAsyncData";

const TYPE_LABELS: Record<string, string> = {
  SingleChoice: "Один вариант",
  MultiChoice: "Несколько вариантов",
  ShortAnswer: "Короткий ответ",
};

export function AdminLessonDetailPage() {
  const { id } = useParams<{ id: string }>();
  const lesson = useAsyncData(() => fetchAdminLesson(id!), [id]);

  const [isAddingQuestion, setIsAddingQuestion] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<AdminQuestionDetailDto | null>(null);
  const [isLoadingQuestion, setIsLoadingQuestion] = useState(false);
  const [deletingQuestionId, setDeletingQuestionId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const openEdit = async (questionId: string) => {
    setIsLoadingQuestion(true);
    try {
      setEditingQuestion(await fetchAdminQuestion(questionId));
    } finally {
      setIsLoadingQuestion(false);
    }
  };

  const handleDeleteQuestion = async () => {
    if (!deletingQuestionId) return;
    setIsDeleting(true);
    setDeleteError(null);

    try {
      await deleteAdminQuestion(deletingQuestionId);
      setDeletingQuestionId(null);
      lesson.reload();
    } catch (error) {
      setDeleteError(extractErrorMessage(error, { 409: "Нельзя удалить вопрос: по нему уже есть ответы студентов." }));
    } finally {
      setIsDeleting(false);
    }
  };

  if (lesson.status === "loading") {
    return <LoadingState label="Загружаем урок…" />;
  }

  if (lesson.status === "error") {
    return <ErrorState onRetry={lesson.reload} />;
  }

  const { data } = lesson;

  return (
    <div>
      <Link to={`/admin/courses/${data.courseId}`} className="mb-2 inline-block text-sm font-medium text-brand-600 hover:text-brand-700">
        ← К курсу
      </Link>
      <PageHeader title={data.title} description="Редактирование урока и вопросов." />

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-[1fr_360px]">
        <div>
          <h2 className="mb-3 text-lg font-bold text-slate-900">Вопросы</h2>

          {data.questions.length === 0 ? (
            <p className="mb-4 text-sm text-slate-500">В уроке пока нет вопросов.</p>
          ) : (
            <div className="mb-4 space-y-2">
              {data.questions.map((question) => (
                <div
                  key={question.id}
                  className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 py-3"
                >
                  <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-slate-100 text-sm font-semibold text-slate-600">
                    {question.order}
                  </span>
                  <button
                    type="button"
                    onClick={() => openEdit(question.id)}
                    className="flex-1 text-left font-medium text-slate-900 hover:text-brand-600"
                  >
                    {question.text}
                  </button>
                  <span className="shrink-0 rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-600">
                    {TYPE_LABELS[question.type]}
                  </span>
                  <button
                    type="button"
                    onClick={() => setDeletingQuestionId(question.id)}
                    className="text-sm font-medium text-red-600 hover:text-red-700"
                  >
                    Удалить
                  </button>
                </div>
              ))}
            </div>
          )}

          <button
            type="button"
            onClick={() => setIsAddingQuestion(true)}
            className="text-sm font-medium text-brand-600 hover:text-brand-700"
          >
            + Добавить вопрос
          </button>
        </div>

        <div>
          <h2 className="mb-3 text-lg font-bold text-slate-900">Параметры урока</h2>
          <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <LessonForm
              initial={data}
              courseId={data.courseId}
              submitLabel="Сохранить"
              onSubmit={async (request) => {
                await updateAdminLesson(data.id, request);
                lesson.reload();
              }}
            />
          </div>
        </div>
      </div>

      <Modal open={isAddingQuestion} onClose={() => setIsAddingQuestion(false)} title="Новый вопрос">
        <QuestionForm
          lessonId={data.id}
          submitLabel="Создать"
          onSubmit={async (request) => {
            await createAdminQuestion(request);
            setIsAddingQuestion(false);
            lesson.reload();
          }}
        />
      </Modal>

      <Modal
        open={editingQuestion !== null || isLoadingQuestion}
        onClose={() => setEditingQuestion(null)}
        title="Редактировать вопрос"
      >
        {isLoadingQuestion && <LoadingState label="Загружаем…" />}
        {editingQuestion && (
          <QuestionForm
            initial={editingQuestion}
            lessonId={data.id}
            submitLabel="Сохранить"
            onSubmit={async (request) => {
              await updateAdminQuestion(editingQuestion.id, request);
              setEditingQuestion(null);
              lesson.reload();
            }}
          />
        )}
      </Modal>

      <ConfirmDialog
        open={deletingQuestionId !== null}
        title="Удалить вопрос?"
        message="Вопрос и его варианты ответа будут удалены безвозвратно."
        error={deleteError}
        isLoading={isDeleting}
        onConfirm={handleDeleteQuestion}
        onCancel={() => {
          setDeletingQuestionId(null);
          setDeleteError(null);
        }}
      />
    </div>
  );
}
