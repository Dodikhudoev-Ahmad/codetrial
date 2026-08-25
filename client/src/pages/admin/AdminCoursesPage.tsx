import { useState } from "react";
import { Link } from "react-router-dom";
import { createAdminCourse, deleteAdminCourse, fetchAdminCourses } from "../../api/admin";
import { extractErrorMessage } from "../../api/errors";
import { CourseForm } from "../../components/admin/CourseForm";
import { ConfirmDialog } from "../../components/ConfirmDialog";
import { LevelBadge } from "../../components/courses/LevelBadge";
import { EmptyState } from "../../components/EmptyState";
import { ErrorState } from "../../components/ErrorState";
import { LoadingState } from "../../components/LoadingState";
import { Modal } from "../../components/Modal";
import { PageHeader } from "../../components/PageHeader";
import { useAsyncData } from "../../hooks/useAsyncData";

export function AdminCoursesPage() {
  const courses = useAsyncData(() => fetchAdminCourses(), []);

  const [isCreating, setIsCreating] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const handleDelete = async () => {
    if (!deletingId) return;
    setIsDeleting(true);
    setDeleteError(null);

    try {
      await deleteAdminCourse(deletingId);
      setDeletingId(null);
      courses.reload();
    } catch (error) {
      setDeleteError(extractErrorMessage(error, { 409: "Нельзя удалить курс: по нему уже есть попытки студентов." }));
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div>
      <div className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <PageHeader title="Админка: курсы" description="Создание и управление курсами, уроками и вопросами." />
        <button
          type="button"
          onClick={() => setIsCreating(true)}
          className="shrink-0 rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700"
        >
          + Новый курс
        </button>
      </div>

      {courses.status === "loading" && <LoadingState label="Загружаем курсы…" />}

      {courses.status === "error" && <ErrorState onRetry={courses.reload} />}

      {courses.status === "success" && courses.data.items.length === 0 && (
        <EmptyState title="Курсов пока нет" description="Создайте первый курс." />
      )}

      {courses.status === "success" && courses.data.items.length > 0 && (
        <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          {courses.data.items.map((course) => (
            <div
              key={course.id}
              className="flex flex-wrap items-center gap-3 border-b border-slate-100 px-5 py-3 last:border-b-0"
            >
              <LevelBadge level={course.level} />
              <Link to={`/admin/courses/${course.id}`} className="flex-1 font-medium text-slate-900 hover:text-brand-600">
                {course.title}
              </Link>
              <span className="text-xs text-slate-500">{course.lessonsCount} уроков</span>
              <span
                className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                  course.isPublished ? "bg-emerald-50 text-emerald-700" : "bg-slate-100 text-slate-500"
                }`}
              >
                {course.isPublished ? "Опубликован" : "Черновик"}
              </span>
              <button
                type="button"
                onClick={() => setDeletingId(course.id)}
                className="text-sm font-medium text-red-600 hover:text-red-700"
              >
                Удалить
              </button>
            </div>
          ))}
        </div>
      )}

      <Modal open={isCreating} onClose={() => setIsCreating(false)} title="Новый курс">
        <CourseForm
          submitLabel="Создать"
          onSubmit={async (request) => {
            await createAdminCourse(request);
            setIsCreating(false);
            courses.reload();
          }}
        />
      </Modal>

      <ConfirmDialog
        open={deletingId !== null}
        title="Удалить курс?"
        message="Курс будет удалён безвозвратно. Если по нему уже есть попытки студентов, удаление будет отклонено."
        error={deleteError}
        isLoading={isDeleting}
        onConfirm={handleDelete}
        onCancel={() => {
          setDeletingId(null);
          setDeleteError(null);
        }}
      />
    </div>
  );
}
