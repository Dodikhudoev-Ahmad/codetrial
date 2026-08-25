import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../auth/useAuth";
import { LoadingState } from "../LoadingState";

// Guards /login and /register: an already-authenticated user is sent to the catalog
// instead of seeing a login form for an account they're already in.
export function RequireGuest() {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingState />;
  }

  if (user) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
