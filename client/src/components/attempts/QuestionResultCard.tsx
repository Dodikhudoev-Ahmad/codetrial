import type { QuestionResultDto } from "../../api/types";
import { CodeBlock } from "../lessons/CodeBlock";

function OptionRow({
  text,
  isCorrectOption,
  wasGiven,
}: {
  text: string;
  isCorrectOption: boolean;
  wasGiven: boolean;
}) {
  const classes = isCorrectOption
    ? "border-emerald-300 bg-emerald-50 text-emerald-800"
    : wasGiven
      ? "border-red-300 bg-red-50 text-red-800"
      : "border-slate-200 text-slate-600";

  return (
    <div className={`flex items-center gap-2 rounded-lg border px-3 py-2 text-sm ${classes}`}>
      <span aria-hidden>{isCorrectOption ? "✓" : wasGiven ? "✕" : "○"}</span>
      <span>{text}</span>
      {wasGiven && !isCorrectOption && <span className="ml-auto text-xs font-medium">Ваш ответ</span>}
    </div>
  );
}

export function QuestionResultCard({ result, index }: { result: QuestionResultDto; index: number }) {
  const givenOptionIds = new Set(result.givenAnswer ? result.givenAnswer.split(",") : []);

  return (
    <div
      className={`rounded-2xl border p-5 ${
        result.isCorrect ? "border-emerald-200 bg-emerald-50/40" : "border-red-200 bg-red-50/40"
      }`}
    >
      <div className="mb-3 flex items-start gap-3">
        <span
          className={`flex h-7 w-7 shrink-0 items-center justify-center rounded-full text-sm font-semibold ${
            result.isCorrect ? "bg-emerald-100 text-emerald-700" : "bg-red-100 text-red-700"
          }`}
        >
          {result.isCorrect ? "✓" : "✕"}
        </span>
        <p className="pt-0.5 text-sm font-medium text-slate-900">
          Вопрос {index}. {result.questionText}
        </p>
      </div>

      {result.codeSnippet && (
        <div className="mb-4">
          <CodeBlock code={result.codeSnippet} />
        </div>
      )}

      {result.options.length > 0 && (
        <div className="mb-3 space-y-1.5">
          {result.options.map((option) => (
            <OptionRow
              key={option.id}
              text={option.text}
              isCorrectOption={option.isCorrect}
              wasGiven={givenOptionIds.has(option.id)}
            />
          ))}
        </div>
      )}

      {result.correctShortAnswer !== null && (
        <div className="mb-3 space-y-1 text-sm">
          <p className="text-slate-600">
            Ваш ответ: <span className="font-medium text-slate-900">{result.givenAnswer || "—"}</span>
          </p>
          {!result.isCorrect && (
            <p className="text-slate-600">
              Правильный ответ: <span className="font-medium text-slate-900">{result.correctShortAnswer}</span>
            </p>
          )}
        </div>
      )}

      <p className="text-sm text-slate-600">{result.explanation}</p>
    </div>
  );
}
