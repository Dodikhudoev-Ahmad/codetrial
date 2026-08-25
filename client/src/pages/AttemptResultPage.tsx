import { useParams } from "react-router-dom";

export function AttemptResultPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <section>
      <h1>Результат попытки</h1>
      <p>Разбор по каждому вопросу (попытка: {id}) появится здесь.</p>
    </section>
  );
}
