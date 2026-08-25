import { useState } from "react";
import { fetchCourses } from "../api/courses";
import type { CourseLevel } from "../api/types";
import { CourseCard } from "../components/courses/CourseCard";
import { CourseFilters } from "../components/courses/CourseFilters";
import { EmptyState } from "../components/EmptyState";
import { ErrorState } from "../components/ErrorState";
import { LoadingState } from "../components/LoadingState";
import { PageHeader } from "../components/PageHeader";
import { Pagination } from "../components/Pagination";
import { useAsyncData } from "../hooks/useAsyncData";
import { useDebouncedValue } from "../hooks/useDebouncedValue";

const PAGE_SIZE = 9;

export function CatalogPage() {
  const [level, setLevel] = useState<CourseLevel | "">("");
  const [language, setLanguage] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  const debouncedLanguage = useDebouncedValue(language, 350);
  const debouncedSearch = useDebouncedValue(search, 350);

  // A stale page number from a wider result set could land past the end of a narrower
  // filtered one, so any filter change resets back to page 1. Done during render
  // (React's documented "adjusting state" pattern) rather than in an effect, so there's
  // no extra render with the stale page number before the reset takes effect.
  const [prevFilters, setPrevFilters] = useState({ level, debouncedLanguage, debouncedSearch });
  if (
    prevFilters.level !== level ||
    prevFilters.debouncedLanguage !== debouncedLanguage ||
    prevFilters.debouncedSearch !== debouncedSearch
  ) {
    setPrevFilters({ level, debouncedLanguage, debouncedSearch });
    setPage(1);
  }

  const courses = useAsyncData(
    () =>
      fetchCourses({
        level: level || undefined,
        language: debouncedLanguage || undefined,
        search: debouncedSearch || undefined,
        page,
        pageSize: PAGE_SIZE,
      }),
    [level, debouncedLanguage, debouncedSearch, page],
  );

  return (
    <div>
      <PageHeader title="Каталог курсов" description="Выберите курс и начните учиться прямо сейчас." />

      <CourseFilters
        level={level}
        language={language}
        search={search}
        onLevelChange={setLevel}
        onLanguageChange={setLanguage}
        onSearchChange={setSearch}
      />

      {courses.status === "loading" && <LoadingState label="Загружаем курсы…" />}

      {courses.status === "error" && <ErrorState onRetry={courses.reload} />}

      {courses.status === "success" && courses.data.items.length === 0 && (
        <EmptyState title="Курсы не найдены" description="Попробуйте изменить фильтры или запрос." />
      )}

      {courses.status === "success" && courses.data.items.length > 0 && (
        <>
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
            {courses.data.items.map((course) => (
              <CourseCard key={course.id} course={course} />
            ))}
          </div>
          <Pagination page={courses.data.page} totalPages={courses.data.totalPages} onPageChange={setPage} />
        </>
      )}
    </div>
  );
}
