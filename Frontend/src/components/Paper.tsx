import clsx from "clsx";
import { ParentComponent, Show } from "solid-js";

interface PaperProps {
  class?: string;
  title?: string;
}

export const Paper: ParentComponent<PaperProps> = (props) => {
  return (
    <div class={clsx("m-6 rounded bg-stone-800 p-6 shadow-xl", props.class)}>
      <Show when={props.title}>
        <h1 class="text-xl">{props.title}</h1>
      </Show>
      {props.children}
    </div>
  );
};
