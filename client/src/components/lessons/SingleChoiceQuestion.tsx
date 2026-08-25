import type { QuestionPreviewDto } from "../../api/types";

interface SingleChoiceQuestionProps {
  question: QuestionPreviewDto;
  value: string;
  onChange: (value: string) => void;
}

export function SingleChoiceQuestion({ question, value, onChange }: SingleChoiceQuestionProps) {
  return (
    <div className="space-y-2">
      {question.options.map((option) => (
        <label
          key={option.id}
          className={`flex cursor-pointer items-center gap-3 rounded-lg border px-4 py-3 text-sm transition-colors ${
            value === option.id ? "border-brand-500 bg-brand-50" : "border-slate-200 hover:bg-slate-50"
          }`}
        >
          <input
            type="radio"
            name={question.id}
            checked={value === option.id}
            onChange={() => onChange(option.id)}
            className="h-4 w-4 accent-brand-600"
          />
          {option.text}
        </label>
      ))}
    </div>
  );
}
