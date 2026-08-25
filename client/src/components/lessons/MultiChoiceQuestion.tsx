import type { QuestionPreviewDto } from "../../api/types";

interface MultiChoiceQuestionProps {
  question: QuestionPreviewDto;
  value: string;
  onChange: (value: string) => void;
}

// GivenAnswer is a comma-separated list of selected option ids, matching
// MultiChoiceAnswerChecker's expected format on the backend.
export function MultiChoiceQuestion({ question, value, onChange }: MultiChoiceQuestionProps) {
  const selected = new Set(value ? value.split(",") : []);

  const toggle = (optionId: string) => {
    const next = new Set(selected);
    if (next.has(optionId)) {
      next.delete(optionId);
    } else {
      next.add(optionId);
    }
    onChange(Array.from(next).join(","));
  };

  return (
    <div className="space-y-2">
      {question.options.map((option) => (
        <label
          key={option.id}
          className={`flex cursor-pointer items-center gap-3 rounded-lg border px-4 py-3 text-sm transition-colors ${
            selected.has(option.id) ? "border-brand-500 bg-brand-50" : "border-slate-200 hover:bg-slate-50"
          }`}
        >
          <input
            type="checkbox"
            checked={selected.has(option.id)}
            onChange={() => toggle(option.id)}
            className="h-4 w-4 rounded accent-brand-600"
          />
          {option.text}
        </label>
      ))}
    </div>
  );
}
