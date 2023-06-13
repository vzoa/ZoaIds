import { HiOutlineArrowLongLeft, HiOutlineEllipsisHorizontal } from "solid-icons/hi";
import { Component, Match, Show, Switch, createSignal } from "solid-js";

interface ExpandableTextProps {
  text: string;
  limit: number;
  rootClass?: string;
  expandedClass?: string;
  condensedClass?: string;
}

export const ExpandableText: Component<ExpandableTextProps> = (props) => {
  const length = () => props.text.length;
  const [expanded, setExpanded] = createSignal(length() < props.limit);
  return (
    <Switch>
      <Match when={length() <= props.limit}>
        <span class={props.rootClass}>{props.text}</span>
      </Match>
      <Match when={length() > props.limit}>
        <span class={props.rootClass}>
          <Show
            when={!expanded()}
            fallback={
              <span class={props.expandedClass}>
                {props.text}
                <HiOutlineArrowLongLeft
                  class="cursor-pointer"
                  onClick={() => setExpanded((s) => !s)}
                />
              </span>
            }
          >
            <span class={props.condensedClass}>{props.text.slice(0, props.limit)}</span>
            <HiOutlineEllipsisHorizontal
              class="cursor-pointer"
              onClick={() => setExpanded((s) => !s)}
            />
          </Show>
        </span>
      </Match>
    </Switch>
  );
};
