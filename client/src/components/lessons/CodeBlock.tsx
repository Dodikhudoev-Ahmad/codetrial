import hljs from "highlight.js/lib/core";
import { useMemo } from "react";
import { highlightLanguages } from "../../utils/highlightLanguages";

for (const [name, language] of Object.entries(highlightLanguages)) {
  if (!hljs.getLanguage(name)) {
    hljs.registerLanguage(name, language);
  }
}

// hljs HTML-escapes the source as part of tokenizing, so this is safe against XSS -
// it never interprets the input as HTML, only as text to highlight.
export function CodeBlock({ code }: { code: string }) {
  const html = useMemo(() => hljs.highlightAuto(code).value, [code]);

  return (
    <pre className="overflow-x-auto rounded-lg bg-[#0d1117] p-4 text-sm">
      <code className="hljs" dangerouslySetInnerHTML={{ __html: html }} />
    </pre>
  );
}
