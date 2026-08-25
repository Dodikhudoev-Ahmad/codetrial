import type { ReactNode } from "react";

export function StatCard({ label, value, suffix }: { label: string; value: ReactNode; suffix?: string }) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <p className="text-sm text-slate-500">{label}</p>
      <p className="mt-1 text-2xl font-bold text-slate-900">
        {value} {suffix && <span className="text-base font-medium text-slate-500">{suffix}</span>}
      </p>
    </div>
  );
}
