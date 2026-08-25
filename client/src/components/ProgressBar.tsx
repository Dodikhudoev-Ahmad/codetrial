export function ProgressBar({ value }: { value: number }) {
  const clamped = Math.min(100, Math.max(0, value));

  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
      <div
        className="h-full rounded-full bg-brand-600 transition-all duration-500"
        style={{ width: `${clamped}%` }}
      />
    </div>
  );
}
