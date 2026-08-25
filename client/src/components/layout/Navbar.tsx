import { Link } from "react-router-dom";
import { useAuth } from "../../auth/useAuth";

export function Navbar() {
  const { user, logout } = useAuth();

  return (
    <header className="navbar">
      <Link to="/" className="navbar__brand">
        CodeTrail
      </Link>

      <nav className="navbar__links">
        <Link to="/">Каталог</Link>
        <Link to="/leaderboard">Рейтинг</Link>
        {user && <Link to="/profile">Профиль</Link>}
        {user?.role === "Admin" && <Link to="/admin">Админка</Link>}
      </nav>

      <div className="navbar__auth">
        {user ? (
          <>
            <span className="navbar__user">{user.displayName}</span>
            <button type="button" onClick={logout}>
              Выйти
            </button>
          </>
        ) : (
          <>
            <Link to="/login">Вход</Link>
            <Link to="/register">Регистрация</Link>
          </>
        )}
      </div>
    </header>
  );
}
