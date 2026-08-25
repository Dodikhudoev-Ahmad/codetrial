import { useParams } from "react-router-dom";

export function LessonPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <section>
      <h1>Урок</h1>
      <p>Теория и тест урока (id: {id}) появятся здесь.</p>
    </section>
  );
}
