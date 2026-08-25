import { useParams } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";

export function AttemptResultPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <PageHeader
      title="Результат попытки"
      description={`Разбор по каждому вопросу (попытка: ${id}) появится здесь.`}
    />
  );
}
