import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <div className="mb-8">
      <h1 className="text-3xl font-extrabold tracking-tight text-slate-900">Страница не найдена</h1>
      <p className="mt-2 text-slate-600">
        <Link to="/" className="font-medium text-brand-600 hover:text-brand-700">
          Вернуться в каталог
        </Link>
      </p>
    </div>
  );
}
