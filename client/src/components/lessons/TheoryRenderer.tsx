import "highlight.js/styles/github-dark.css";
import ReactMarkdown from "react-markdown";
import rehypeHighlight from "rehype-highlight";
import remarkGfm from "remark-gfm";
import { highlightLanguages } from "../../utils/highlightLanguages";

const rehypeHighlightOptions = { languages: highlightLanguages, detect: true };

export function TheoryRenderer({ markdown }: { markdown: string }) {
  return (
    <div className="prose prose-slate max-w-none prose-headings:font-bold prose-a:text-brand-600 prose-code:rounded prose-code:bg-slate-100 prose-code:px-1 prose-code:py-0.5 prose-code:font-normal prose-code:before:content-none prose-code:after:content-none prose-pre:bg-[#0d1117]">
      <ReactMarkdown remarkPlugins={[remarkGfm]} rehypePlugins={[[rehypeHighlight, rehypeHighlightOptions]]}>
        {markdown}
      </ReactMarkdown>
    </div>
  );
}
