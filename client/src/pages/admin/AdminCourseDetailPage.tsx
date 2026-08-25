import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { createAdminLesson, deleteAdminLesson, fetchAdminCourse, fetchAdminCourseStats, updateAdminCourse } from "../../api/admin";
import { extractErrorMessage } from "../../api/errors";
import { CourseForm } from "../../components/admin/CourseForm";
import { ConfirmDialog } from "../../components/ConfirmDialog";
import { ErrorState } from "../../components/ErrorState";
import { LoadingState } from "../../components/LoadingState";
import { Modal } from "../../components/Modal";
import { PageHeader } from "../../components/PageHeader";
import { LessonForm } from "../../components/admin/LessonForm";
import { useAsyncData } from "../../hooks/useAsyncData";

export function AdminCourseDetailPage() {
  const { id } = useParams<{ id: string }>();
  const course = useAsyncData(() => fetchAdminCourse(id!), [id]);
  const stats = useAsyncData(() => fetchAdminCourseStats(id!), [id, course.status]);

  const [isAddingLesson, setIsAddingLesson] = useState(false);
  const [deletingLessonId, setDeletingLessonId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const handleDeleteLesson = async () => {
    if (!deletingLessonId) return;
    setIsDeleting(true);
    setDeleteError(null);

    try {
      await deleteAdminLesson(deletingLessonId);
      setDeletingLessonId(null);
      course.reload();
    } catch (error) {
      setDeleteError(extractErrorMessage(error, { 409: "Нельзя удалить урок: по нему уже есть попытки студентов." }));
    } finally {
      setIsDeleting(false);
    }
  };

  if (course.status === "loading") {
    return <LoadingState label="Загружаем курс…" />;
  }

  if (course.status === "error") {
    return <ErrorState onRetry={course.reload} />;
  }

  const { data } = course;

  return (
    <div>
      <Link to="/admin" className="mb-2 inline-block text-sm font-medium text-brand-600 hover:text-brand-700">
        ← Все курсы
      </Link>
      <PageHeader title={data.title} description="Редактирование курса, уроков и статистика." />

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-[1fr_320px]">
        <div>
          <h2 className="mb-3 text-lg font-bold text-slate-900">Уроки</h2>

          {data.lessons.length === 0 ? (
            <p className="mb-4 text-sm text-slate-500">В курсе пока нет уроков.</p>
          ) : (
            <div className="mb-4 space-y-2">
              {data.lessons.map((lesson) => (
                <div
                  key={lesson.id}
                  className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 py-3"
                >
                  <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-slate-100 text-sm font-semibold text-slate-600">
                    {lesson.order}
                  </span>
                  <Link to={`/admin/lessons/${lesson.id}`} className="flex-1 font-medium text-slate-900 hover:text-brand-600">
                    {lesson.title}
                  </Link>
                  <span className="text-xs text-slate-500">
                    {lesson.questionsCount} вопросов · {lesson.xpReward} XP
                  </span>
                  <button
                    type="button"
                    onClick={() => setDeletingLessonId(lesson.id)}
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
            onClick={() => setIsAddingLesson(true)}
            className="mb-8 text-sm font-medium text-brand-600 hover:text-brand-700"
          >
            + Добавить урок
          </button>

          <h2 className="mb-3 text-lg font-bold text-slate-900">Параметры курса</h2>
          <div className="max-w-lg rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <CourseForm
              initial={data}
              submitLabel="Сохранить"
              onSubmit={async (request) => {
                await updateAdminCourse(data.id, request);
                course.reload();
              }}
            />
          </div>
        </div>

        <div>
          <h2 className="mb-3 text-lg font-bold text-slate-900">Статистика</h2>

          {stats.status === "loading" && <LoadingState label="Считаем…" />}
          {stats.status === "error" && <ErrorState onRetry={stats.reload} />}

          {stats.status === "success" && (
            <div className="space-y-3">
              <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                <p className="text-xs text-slate-500">Записались</p>
                <p className="text-xl font-bold text-slate-900">{stats.data.enrollmentsCount}</p>
              </div>
              <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                <p className="text-xs text-slate-500">Завершили курс</p>
                <p className="text-xl font-bold text-slate-900">{stats.data.completionsCount}</p>
              </div>
              <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                <p className="text-xs text-slate-500">Средний балл попыток</p>
                <p className="text-xl font-bold text-slate-900">{stats.data.averageScorePercent}%</p>
              </div>

              {stats.data.lessons.length > 0 && (
                <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                  <p className="mb-2 text-xs font-medium text-slate-500">По урокам</p>
                  <div className="space-y-2">
                    {stats.data.lessons.map((lessonStats) => (
                      <div key={lessonStats.lessonId} className="text-sm">
                        <p className="font-medium text-slate-800">{lessonStats.lessonTitle}</p>
                        <p className="text-xs text-slate-500">
                          {lessonStats.attemptsCount} попыток · {lessonStats.studentsPassedCount} прошли · ср.{" "}
                          {lessonStats.averageScorePercent}%
                        </p>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>

      <Modal open={isAddingLesson} onClose={() => setIsAddingLesson(false)} title="Новый урок">
        <LessonForm
          courseId={data.id}
          submitLabel="Создать"
          onSubmit={async (request) => {
            await createAdminLesson(request);
            setIsAddingLesson(false);
            course.reload();
          }}
        />
      </Modal>

      <ConfirmDialog
        open={deletingLessonId !== null}
        title="Удалить урок?"
        message="Урок и все его вопросы будут удалены безвозвратно."
        error={deleteError}
        isLoading={isDeleting}
        onConfirm={handleDeleteLesson}
        onCancel={() => {
          setDeletingLessonId(null);
          setDeleteError(null);
        }}
      />
    </div>
  );
}
