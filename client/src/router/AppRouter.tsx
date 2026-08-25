import { lazy, Suspense } from "react";
import { Route, Routes } from "react-router-dom";
import { Layout } from "../components/layout/Layout";
import { LoadingState } from "../components/LoadingState";
import { RequireAuth } from "../components/routing/RequireAuth";
import { RequireGuest } from "../components/routing/RequireGuest";
import { RequireRole } from "../components/routing/RequireRole";
import { AdminPage } from "../pages/AdminPage";
import { AttemptResultPage } from "../pages/AttemptResultPage";
import { CatalogPage } from "../pages/CatalogPage";
import { CourseDetailPage } from "../pages/CourseDetailPage";
import { LeaderboardPage } from "../pages/LeaderboardPage";
import { LoginPage } from "../pages/LoginPage";
import { NotFoundPage } from "../pages/NotFoundPage";
import { ProfilePage } from "../pages/ProfilePage";
import { RegisterPage } from "../pages/RegisterPage";

// The markdown/syntax-highlighting stack (react-markdown, rehype-highlight, lowlight)
// is only needed here, and it's heavy - splitting it into its own chunk keeps the
// catalog/login/etc. bundle light for visitors who never open a lesson.
const LessonPage = lazy(() => import("../pages/LessonPage").then((m) => ({ default: m.LessonPage })));

export function AppRouter() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Public: catalog, course description and leaderboard are open to guests. */}
        <Route index element={<CatalogPage />} />
        <Route path="courses/:slug" element={<CourseDetailPage />} />
        <Route path="leaderboard" element={<LeaderboardPage />} />

        {/* An already-authenticated user has no reason to see a login/register form. */}
        <Route element={<RequireGuest />}>
          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<RegisterPage />} />
        </Route>

        {/* Student-only in practice (the API enforces the role), but any authenticated
            user reaching these gets a real page instead of a silent redirect - the
            API's own 403 surfaces as an error state once these pages fetch data. */}
        <Route element={<RequireAuth />}>
          <Route
            path="lessons/:id"
            element={
              <Suspense fallback={<LoadingState label="Загружаем урок…" />}>
                <LessonPage />
              </Suspense>
            }
          />
          <Route path="attempts/:id" element={<AttemptResultPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>

        <Route element={<RequireRole role="Admin" />}>
          <Route path="admin" element={<AdminPage />} />
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
