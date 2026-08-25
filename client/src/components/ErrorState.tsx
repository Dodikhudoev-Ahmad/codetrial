interface ErrorStateProps {
  message?: string;
  onRetry?: () => void;
}

export function ErrorState({ message = "Не удалось загрузить данные.", onRetry }: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center gap-3 rounded-2xl border border-red-100 bg-red-50 px-6 py-10 text-center">
      <p className="text-sm font-medium text-red-700">{message}</p>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="rounded-lg border border-red-200 bg-white px-4 py-1.5 text-sm font-medium text-red-700 transition-colors hover:bg-red-100"
        >
          Повторить попытку
        </button>
      )}
    </div>
  );
}
