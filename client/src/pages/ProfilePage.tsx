import { fetchProgress } from "../api/profile";
import { useAuth } from "../auth/useAuth";
import { CourseProgressRow } from "../components/profile/CourseProgressRow";
import { StatCard } from "../components/profile/StatCard";
import { EmptyState } from "../components/EmptyState";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { PageHeader } from "../components/PageHeader";
import { useAsyncData } from "../hooks/useAsyncData";
import { formatDate } from "../utils/formatDate";
import { pluralizeRu } from "../utils/pluralizeRu";

export function ProfilePage() {
  const { user } = useAuth();
  const progress = useAsyncData(() => fetchProgress(), []);

  if (!user) {
    return null;
  }

  return (
    <div>
      <PageHeader title="Профиль" description="Ваш прогресс, опыт и серия дней." />

      <div className="mb-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <StatCard label="Опыт" value={user.totalXp} suffix="XP" />
        <StatCard
          label="Серия дней"
          value={user.currentStreak}
          suffix={pluralizeRu(user.currentStreak, ["день", "дня", "дней"])}
        />
        <StatCard label="Последняя активность" value={user.lastActivityDate ? formatDate(user.lastActivityDate) : "—"} />
      </div>

      <h2 className="mb-4 text-xl font-bold text-slate-900">Мои курсы</h2>

      {progress.status === "loading" && <LoadingState label="Загружаем прогресс…" />}

      {progress.status === "error" && <ErrorState onRetry={progress.reload} />}

      {progress.status === "success" && progress.data.length === 0 && (
        <EmptyState title="Вы ещё не записаны ни на один курс" description="Загляните в каталог, чтобы начать." />
      )}

      {progress.status === "success" && progress.data.length > 0 && (
        <div className="space-y-3">
          {progress.data.map((course) => (
            <CourseProgressRow key={course.courseId} course={course} />
          ))}
        </div>
      )}
    </div>
  );
}
