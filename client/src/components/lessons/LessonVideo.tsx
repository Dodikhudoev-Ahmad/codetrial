import { useEffect, useId, useRef } from "react";

// Minimal surface of the YouTube IFrame Player API this component actually uses -
// https://developers.google.com/youtube/iframe_api_reference doesn't ship types.
interface YouTubePlayer {
  getCurrentTime(): number;
  getDuration(): number;
  destroy(): void;
}

interface YouTubeNamespace {
  Player: new (
    elementId: string,
    options: {
      videoId: string;
      host: string;
      events: {
        onStateChange: (event: { data: number; target: YouTubePlayer }) => void;
      };
    },
  ) => YouTubePlayer;
  PlayerState: { PLAYING: number; PAUSED: number; ENDED: number };
}

declare global {
  interface Window {
    YT?: YouTubeNamespace;
    onYouTubeIframeAPIReady?: () => void;
  }
}

// The IFrame API script calls a single global callback once loaded - shared across
// every LessonVideo instance so it's only ever requested once per page.
let apiLoadPromise: Promise<YouTubeNamespace> | null = null;

function loadYouTubeApi(): Promise<YouTubeNamespace> {
  if (window.YT) return Promise.resolve(window.YT);

  apiLoadPromise ??= new Promise((resolve) => {
    const previous = window.onYouTubeIframeAPIReady;
    window.onYouTubeIframeAPIReady = () => {
      previous?.();
      resolve(window.YT!);
    };
    const script = document.createElement("script");
    script.src = "https://www.youtube.com/iframe_api";
    document.head.appendChild(script);
  });

  return apiLoadPromise;
}

interface LessonVideoProps {
  videoId: string;
  title: string;
  onProgress: (watchedPercent: number) => void;
}

const POLL_INTERVAL_MS = 2000;

export function LessonVideo({ videoId, title, onProgress }: LessonVideoProps) {
  const elementId = `yt-player-${useId().replace(/[^a-zA-Z0-9]/g, "")}`;
  const onProgressRef = useRef(onProgress);
  onProgressRef.current = onProgress;

  useEffect(() => {
    let player: YouTubePlayer | null = null;
    let pollHandle: ReturnType<typeof setInterval> | null = null;
    let cancelled = false;

    const stopPolling = () => {
      if (pollHandle !== null) {
        clearInterval(pollHandle);
        pollHandle = null;
      }
    };

    const reportProgress = () => {
      if (!player) return;
      const duration = player.getDuration();
      if (!duration) return;
      const percent = Math.min(100, Math.round((player.getCurrentTime() / duration) * 100));
      onProgressRef.current(percent);
    };

    loadYouTubeApi().then((YT) => {
      if (cancelled) return;

      player = new YT.Player(elementId, {
        videoId,
        host: "https://www.youtube-nocookie.com",
        events: {
          onStateChange: (event) => {
            if (event.data === YT.PlayerState.PLAYING) {
              stopPolling();
              pollHandle = setInterval(reportProgress, POLL_INTERVAL_MS);
            } else {
              stopPolling();
              if (event.data === YT.PlayerState.PAUSED || event.data === YT.PlayerState.ENDED) {
                reportProgress();
              }
            }
          },
        },
      });
    });

    return () => {
      cancelled = true;
      stopPolling();
      player?.destroy();
    };
    // videoId/elementId are stable for the lifetime of a mounted lesson page.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [videoId]);

  return (
    <div className="mb-8 overflow-hidden rounded-xl bg-slate-950 shadow-sm" style={{ aspectRatio: "16 / 9" }}>
      <div id={elementId} title={title} className="h-full w-full" />
    </div>
  );
}
