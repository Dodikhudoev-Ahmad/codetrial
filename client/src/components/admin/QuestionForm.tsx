import { useId, useState, type FormEvent } from "react";
import { extractErrorMessage } from "../../api/errors";
import type { AdminQuestionDetailDto, QuestionType, UpsertQuestionRequest } from "../../api/types";
import { ErrorBanner } from "../form/ErrorBanner";
import { SubmitButton } from "../form/SubmitButton";
import { TextField } from "../form/TextField";

interface OptionDraft {
  text: string;
  isCorrect: boolean;
}

interface QuestionFormProps {
  initial?: AdminQuestionDetailDto;
  lessonId: string;
  submitLabel: string;
  onSubmit: (request: UpsertQuestionRequest) => Promise<void>;
}

const fieldClasses =
  "w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition-colors focus:border-brand-500 focus:ring-2 focus:ring-brand-100";

export function QuestionForm({ initial, lessonId, submitLabel, onSubmit }: QuestionFormProps) {
  const formId = useId();
  const [type, setType] = useState<QuestionType>(initial?.type ?? "SingleChoice");
  const [text, setText] = useState(initial?.text ?? "");
  const [codeSnippet, setCodeSnippet] = useState(initial?.codeSnippet ?? "");
  const [explanation, setExplanation] = useState(initial?.explanation ?? "");
  const [options, setOptions] = useState<OptionDraft[]>(
    initial && initial.options.length > 0
      ? initial.options.map((o) => ({ text: o.text, isCorrect: o.isCorrect }))
      : [
          { text: "", isCorrect: false },
          { text: "", isCorrect: false },
        ],
  );
  const [expectedAnswer, setExpectedAnswer] = useState(initial?.expectedAnswer ?? "");
  const [isCaseSensitive, setIsCaseSensitive] = useState(initial?.isCaseSensitive ?? false);
  const [serverError, setServerError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const updateOption = (index: number, patch: Partial<OptionDraft>) => {
    setOptions((prev) => prev.map((option, i) => (i === index ? { ...option, ...patch } : option)));
  };

  const setSingleCorrect = (index: number) => {
    setOptions((prev) => prev.map((option, i) => ({ ...option, isCorrect: i === index })));
  };

  const addOption = () => setOptions((prev) => [...prev, { text: "", isCorrect: false }]);
  const removeOption = (index: number) => setOptions((prev) => prev.filter((_, i) => i !== index));

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setServerError(null);
    setIsSubmitting(true);

    try {
      await onSubmit({
        lessonId,
        type,
        text,
        codeSnippet: codeSnippet.trim() ? codeSnippet : null,
        explanation,
        options: type === "ShortAnswer" ? [] : options.filter((option) => option.text.trim().length > 0),
        expectedAnswer: type === "ShortAnswer" ? expectedAnswer : null,
        isCaseSensitive,
      });
    } catch (error) {
      setServerError(
        extractErrorMessage(error, {
          400: "Проверьте варианты ответов: нужно минимум два варианта и хотя бы один правильный (ровно один — для вопроса с одним ответом).",
        }),
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <ErrorBanner message={serverError} />

      <div>
        <label htmlFor={`${formId}-type`} className="mb-1 block text-sm font-medium text-slate-700">
          Тип вопроса
        </label>
        <select
          id={`${formId}-type`}
          value={type}
          onChange={(event) => setType(event.target.value as QuestionType)}
          className={fieldClasses}
        >
          <option value="SingleChoice">Один вариант ответа</option>
          <option value="MultiChoice">Несколько вариантов ответа</option>
          <option value="ShortAnswer">Короткий текстовый ответ</option>
        </select>
      </div>

      <div>
        <label htmlFor={`${formId}-text`} className="mb-1 block text-sm font-medium text-slate-700">
          Текст вопроса
        </label>
        <textarea
          id={`${formId}-text`}
          required
          rows={2}
          value={text}
          onChange={(event) => setText(event.target.value)}
          className={fieldClasses}
        />
      </div>

      <div>
        <label htmlFor={`${formId}-codeSnippet`} className="mb-1 block text-sm font-medium text-slate-700">
          Код (необязательно)
        </label>
        <textarea
          id={`${formId}-codeSnippet`}
          rows={3}
          value={codeSnippet}
          onChange={(event) => setCodeSnippet(event.target.value)}
          className={`${fieldClasses} font-mono`}
        />
      </div>

      <div>
        <label htmlFor={`${formId}-explanation`} className="mb-1 block text-sm font-medium text-slate-700">
          Пояснение (показывается после ответа)
        </label>
        <textarea
          id={`${formId}-explanation`}
          required
          rows={2}
          value={explanation}
          onChange={(event) => setExplanation(event.target.value)}
          className={fieldClasses}
        />
      </div>

      {(type === "SingleChoice" || type === "MultiChoice") && (
        <div>
          <p className="mb-2 text-sm font-medium text-slate-700">Варианты ответа</p>
          <div className="space-y-2">
            {options.map((option, index) => (
              <div key={index} className="flex items-center gap-2">
                <input
                  type={type === "SingleChoice" ? "radio" : "checkbox"}
                  name={`${formId}-correct-option`}
                  checked={option.isCorrect}
                  onChange={() =>
                    type === "SingleChoice" ? setSingleCorrect(index) : updateOption(index, { isCorrect: !option.isCorrect })
                  }
                  className="h-4 w-4 accent-brand-600"
                  aria-label="Правильный вариант"
                />
                <input
                  type="text"
                  required
                  value={option.text}
                  onChange={(event) => updateOption(index, { text: event.target.value })}
                  placeholder={`Вариант ${index + 1}`}
                  className={`flex-1 ${fieldClasses}`}
                />
                {options.length > 2 && (
                  <button
                    type="button"
                    onClick={() => removeOption(index)}
                    className="text-slate-400 transition-colors hover:text-red-600"
                    aria-label="Удалить вариант"
                  >
                    ✕
                  </button>
                )}
              </div>
            ))}
          </div>
          <button
            type="button"
            onClick={addOption}
            className="mt-2 text-sm font-medium text-brand-600 hover:text-brand-700"
          >
            + Добавить вариант
          </button>
        </div>
      )}

      {type === "ShortAnswer" && (
        <>
          <TextField
            id={`${formId}-expectedAnswer`}
            label="Правильный ответ"
            required
            value={expectedAnswer}
            onChange={(event) => setExpectedAnswer(event.target.value)}
          />
          <label className="flex items-center gap-2 text-sm text-slate-700">
            <input
              type="checkbox"
              checked={isCaseSensitive}
              onChange={(event) => setIsCaseSensitive(event.target.checked)}
              className="h-4 w-4 rounded accent-brand-600"
            />
            Учитывать регистр
          </label>
        </>
      )}

      <SubmitButton isLoading={isSubmitting}>{submitLabel}</SubmitButton>
    </form>
  );
}
