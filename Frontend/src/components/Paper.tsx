import clsx from "clsx";
import { ParentComponent } from "solid-js";

interface PaperProps {
  class?: string;
}

export const Paper: ParentComponent<PaperProps> = (props) => {
  return (
    <div class={clsx("m-6 rounded bg-stone-800 p-6 shadow-xl", props.class)}>{props.children}</div>
  );
};
