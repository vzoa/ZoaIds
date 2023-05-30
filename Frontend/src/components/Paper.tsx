import { ParentComponent } from "solid-js";

export const Paper: ParentComponent = (props) => {
  return <div class="m-6 rounded bg-stone-800 p-6 shadow-xl">{props.children}</div>;
};
