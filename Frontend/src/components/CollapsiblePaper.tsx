import { Collapsible } from "@kobalte/core";
import clsx from "clsx";
import { ParentComponent } from "solid-js";

interface PaperProps {
  title: string;
  class?: string;
}

export const CollapsiblePaper: ParentComponent<PaperProps> = (props) => {
  return (
    <Collapsible.Root class={clsx("m-6 rounded bg-stone-800 p-6 shadow-xl", props.class)}>
      <Collapsible.Trigger class="collapsible__trigger">
        <h1 class="text-xl">{props.title}</h1>
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke-width="1.5"
          stroke="currentColor"
          class="collapsible__trigger-icon"
        >
          <path stroke-linecap="round" stroke-linejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" />
        </svg>
      </Collapsible.Trigger>
      <Collapsible.Content class="collapsible__content">{props.children}</Collapsible.Content>
    </Collapsible.Root>
  );
};
