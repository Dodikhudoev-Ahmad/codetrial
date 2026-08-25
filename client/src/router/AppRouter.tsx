import { Route, Routes } from "react-router-dom";
import { Layout } from "../components/layout/Layout";
import { AdminPage } from "../pages/AdminPage";
import { AttemptResultPage } from "../pages/AttemptResultPage";
import { CatalogPage } from "../pages/CatalogPage";
import { CourseDetailPage } from "../pages/CourseDetailPage";
import { LeaderboardPage } from "../pages/LeaderboardPage";
import { LessonPage } from "../pages/LessonPage";
import { LoginPage } from "../pages/LoginPage";
import { NotFoundPage } from "../pages/NotFoundPage";
import { ProfilePage } from "../pages/ProfilePage";
import { RegisterPage } from "../pages/RegisterPage";

// No route guards yet - protected routes and the login/register forms themselves are
// next (day 9). Today's job is just the scaffold: every page from the spec has a route.
export function AppRouter() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<CatalogPage />} />
        <Route path="courses/:slug" element={<CourseDetailPage />} />
        <Route path="lessons/:id" element={<LessonPage />} />
        <Route path="attempts/:id" element={<AttemptResultPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="leaderboard" element={<LeaderboardPage />} />
        <Route path="admin" element={<AdminPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
