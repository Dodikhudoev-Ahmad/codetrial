import type { ButtonHTMLAttributes } from "react";

interface SubmitButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  isLoading?: boolean;
  loadingLabel?: string;
}

export function SubmitButton({
  isLoading,
  loadingLabel = "Загрузка…",
  children,
  disabled,
  ...rest
}: SubmitButtonProps) {
  return (
    <button
      type="submit"
      disabled={isLoading || disabled}
      className="w-full rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white shadow-sm shadow-brand-600/20 transition-colors hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-60"
      {...rest}
    >
      {isLoading ? loadingLabel : children}
    </button>
  );
}
