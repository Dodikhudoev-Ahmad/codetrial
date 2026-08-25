import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../../auth/useAuth";
import { LoadingState } from "../LoadingState";

// Guards "student" pages: an unauthenticated visitor is redirected to /login,
// remembering where they were headed so LoginPage can send them back after success.
export function RequireAuth() {
  const { user, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return <LoadingState />;
  }

  if (!user) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return <Outlet />;
}
