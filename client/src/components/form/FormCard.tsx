import type { ReactNode } from "react";

export function FormCard({ children }: { children: ReactNode }) {
  return <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">{children}</div>;
}
