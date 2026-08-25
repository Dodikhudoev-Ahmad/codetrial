import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <section>
      <h1>Страница не найдена</h1>
      <p>
        <Link to="/">Вернуться в каталог</Link>
      </p>
    </section>
  );
}
