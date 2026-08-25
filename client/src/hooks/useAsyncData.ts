import { useCallback, useEffect, useState } from "react";

type AsyncState<T> = { status: "loading" } | { status: "error" } | { status: "success"; data: T };

// Every list/detail page in the app follows the same shape: fetch on mount and again
// whenever some set of inputs changes, track loading/error/success, and offer a manual
// retry. Centralizing it here means that shape (and the explicit loading/error states
// the spec requires) doesn't get re-implemented slightly differently on every page.
export function useAsyncData<T>(fetcher: () => Promise<T>, deps: unknown[]) {
  const [reloadToken, setReloadToken] = useState(0);
  const [state, setState] = useState<AsyncState<T>>({ status: "loading" });

  useEffect(() => {
    let cancelled = false;
    setState({ status: "loading" });

    fetcher()
      .then((data) => {
        if (!cancelled) setState({ status: "success", data });
      })
      .catch(() => {
        if (!cancelled) setState({ status: "error" });
      });

    return () => {
      cancelled = true;
    };
    // fetcher is intentionally excluded: callers pass a fresh closure each render, and
    // re-running only when `deps` (plus reloadToken) changes is the whole point.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, reloadToken]);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  return { ...state, reload };
}
