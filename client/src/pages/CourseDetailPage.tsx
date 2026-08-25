import { useParams } from "react-router-dom";

export function CourseDetailPage() {
  const { slug } = useParams<{ slug: string }>();

  return (
    <section>
      <h1>Страница курса</h1>
      <p>Описание курса и оглавление уроков (курс: {slug}) появятся здесь.</p>
    </section>
  );
}
