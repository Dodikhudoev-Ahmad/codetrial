interface LessonVideoProps {
  videoId: string;
  title: string;
}

export function LessonVideo({ videoId, title }: LessonVideoProps) {
  return (
    <div className="mb-8 overflow-hidden rounded-xl bg-slate-950 shadow-sm" style={{ aspectRatio: "16 / 9" }}>
      <iframe
        className="h-full w-full"
        src={`https://www.youtube-nocookie.com/embed/${videoId}`}
        title={title}
        allow="accelerometer; encrypted-media; gyroscope; picture-in-picture"
        allowFullScreen
        loading="lazy"
        referrerPolicy="strict-origin-when-cross-origin"
      />
    </div>
  );
}
