import { Navigate, Outlet } from "react-router-dom";
import type { UserRole } from "../../api/types";
import { useAuth } from "../../auth/useAuth";
import { LoadingState } from "../LoadingState";

// Guards role-restricted pages (e.g. /admin): a student who reaches this route directly
// is sent back to the catalog rather than shown an empty/broken admin screen.
export function RequireRole({ role }: { role: UserRole }) {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingState />;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (user.role !== role) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
