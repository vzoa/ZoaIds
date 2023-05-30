import DOMPurify from "dompurify";
import MarkdownIt from "markdown-it";
import { Component } from "solid-js";

export const MarkdownRender: Component<{ markdown: string }> = (props) => {
  let md = new MarkdownIt({ html: true });
  return <div class="markdown" innerHTML={DOMPurify.sanitize(md.render(props.markdown))} />;
};
