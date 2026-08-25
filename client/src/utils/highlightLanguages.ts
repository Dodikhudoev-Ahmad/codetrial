// The full highlight.js package registers ~190 languages and dominates the bundle
// (500+ kB gzipped). This project's course content is C#/SQL today with room for
// common web languages later - registering just those keeps the bundle reasonable.
import bash from "highlight.js/lib/languages/bash";
import cs from "highlight.js/lib/languages/csharp";
import css from "highlight.js/lib/languages/css";
import javascript from "highlight.js/lib/languages/javascript";
import json from "highlight.js/lib/languages/json";
import sql from "highlight.js/lib/languages/sql";
import typescript from "highlight.js/lib/languages/typescript";
import xml from "highlight.js/lib/languages/xml";

export const highlightLanguages = {
  csharp: cs,
  cs,
  sql,
  javascript,
  js: javascript,
  typescript,
  ts: typescript,
  json,
  xml,
  html: xml,
  css,
  bash,
  shell: bash,
};
