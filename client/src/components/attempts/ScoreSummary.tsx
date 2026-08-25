import { PASSING_SCORE_PERCENT, type AttemptResultDto } from "../../api/types";
import { ProgressBar } from "../ProgressBar";

export function ScoreSummary({ result }: { result: AttemptResultDto }) {
  return (
    <div
      className={`mb-8 rounded-2xl border p-6 ${
        result.isPassed ? "border-emerald-200 bg-emerald-50" : "border-amber-200 bg-amber-50"
      }`}
    >
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div className="min-w-[200px] flex-1">
          <p className={`text-sm font-medium ${result.isPassed ? "text-emerald-700" : "text-amber-700"}`}>
            {result.isPassed ? "Урок пройден!" : "Порог не достигнут"}
          </p>
          <p className="text-4xl font-extrabold tracking-tight text-slate-900">{result.scorePercent}%</p>
          <p className="mt-1 text-sm text-slate-600">
            Попытка №{result.attemptNumber} · нужно набрать {PASSING_SCORE_PERCENT}%, чтобы засчитать урок
          </p>
          <div className="mt-3 max-w-xs">
            <ProgressBar value={result.scorePercent} />
          </div>
        </div>

        {result.xpAwarded > 0 && (
          <div className="rounded-xl bg-white px-4 py-3 text-center shadow-sm">
            <p className="text-2xl font-bold text-brand-600">+{result.xpAwarded}</p>
            <p className="text-xs font-medium text-slate-500">опыта</p>
          </div>
        )}
      </div>
    </div>
  );
}
