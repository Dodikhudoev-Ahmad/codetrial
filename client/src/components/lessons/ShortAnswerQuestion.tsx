interface ShortAnswerQuestionProps {
  value: string;
  onChange: (value: string) => void;
}

export function ShortAnswerQuestion({ value, onChange }: ShortAnswerQuestionProps) {
  return (
    <input
      type="text"
      value={value}
      onChange={(event) => onChange(event.target.value)}
      placeholder="Введите ответ…"
      className="w-full max-w-md rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100"
    />
  );
}
