import type { ReactNode } from "react";
import type { QuestionPreviewDto } from "../../api/types";
import { CodeBlock } from "./CodeBlock";

interface QuestionCardProps {
  index: number;
  question: QuestionPreviewDto;
  children: ReactNode;
}

export function QuestionCard({ index, question, children }: QuestionCardProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="mb-3 flex items-start gap-3">
        <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-brand-100 text-sm font-semibold text-brand-700">
          {index}
        </span>
        <p className="pt-0.5 text-sm font-medium text-slate-900">{question.text}</p>
      </div>

      {question.codeSnippet && (
        <div className="mb-4">
          <CodeBlock code={question.codeSnippet} />
        </div>
      )}

      {children}
    </div>
  );
}
