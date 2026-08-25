import { useParams } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";

export function CourseDetailPage() {
  const { slug } = useParams<{ slug: string }>();

  return (
    <PageHeader
      title="Страница курса"
      description={`Описание курса и оглавление уроков (курс: ${slug}) появятся здесь.`}
    />
  );
}
