import { useParams } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";

export function LessonPage() {
  const { id } = useParams<{ id: string }>();

  return <PageHeader title="Урок" description={`Теория и тест урока (id: ${id}) появятся здесь.`} />;
}
