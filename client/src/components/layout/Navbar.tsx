import { NavLink } from "react-router-dom";
import { useAuth } from "../../auth/useAuth";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `text-sm font-medium transition-colors ${
    isActive ? "text-brand-700" : "text-slate-600 hover:text-brand-600"
  }`;

export function Navbar() {
  const { user, logout } = useAuth();

  return (
    <header className="sticky top-0 z-10 border-b border-slate-200/70 bg-white/80 backdrop-blur">
      <div className="mx-auto flex max-w-5xl flex-wrap items-center gap-x-8 gap-y-3 px-6 py-4">
        <NavLink
          to="/"
          className="bg-gradient-to-r from-brand-600 to-brand-400 bg-clip-text text-lg font-extrabold tracking-tight text-transparent"
        >
          CodeTrail
        </NavLink>

        <nav className="flex flex-wrap items-center gap-6">
          <NavLink to="/" end className={navLinkClass}>
            Каталог
          </NavLink>
          <NavLink to="/leaderboard" className={navLinkClass}>
            Рейтинг
          </NavLink>
          {user && (
            <NavLink to="/profile" className={navLinkClass}>
              Профиль
            </NavLink>
          )}
          {user?.role === "Admin" && (
            <NavLink to="/admin" className={navLinkClass}>
              Админка
            </NavLink>
          )}
        </nav>

        <div className="ml-auto flex items-center gap-3">
          {user ? (
            <>
              <span className="text-sm font-medium text-slate-700">{user.displayName}</span>
              <button
                type="button"
                onClick={logout}
                className="rounded-full border border-slate-200 px-4 py-1.5 text-sm font-medium text-slate-600 transition-colors hover:border-slate-300 hover:bg-slate-50"
              >
                Выйти
              </button>
            </>
          ) : (
            <>
              <NavLink to="/login" className="text-sm font-medium text-slate-600 hover:text-brand-600">
                Вход
              </NavLink>
              <NavLink
                to="/register"
                className="rounded-full bg-brand-600 px-4 py-1.5 text-sm font-semibold text-white shadow-sm shadow-brand-600/30 transition-colors hover:bg-brand-700"
              >
                Регистрация
              </NavLink>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
