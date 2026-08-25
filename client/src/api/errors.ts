import axios from "axios";

// Backend error messages (CodeTrail.Application's AppException-derived types) are in
// English - internal/technical text, never meant for display. The UI is in Russian, so
// callers map the HTTP status codes they expect to a localized message; anything
// unmapped (network failure, 500, etc.) gets the generic fallback. This never surfaces
// raw backend text to the user.
export function extractErrorMessage(
  error: unknown,
  statusMessages: Partial<Record<number, string>> = {},
  fallback = "Что-то пошло не так. Попробуйте ещё раз.",
): string {
  if (axios.isAxiosError(error)) {
    const status = error.response?.status;
    if (status && statusMessages[status]) {
      return statusMessages[status];
    }
  }

  return fallback;
}
