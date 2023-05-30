import { Component, JSXElement } from "solid-js";
import { Button } from "@kobalte/core";

interface SubmitButtonProps {
  text: string;
  icon: JSXElement;
}

export const SubmitButton: Component<SubmitButtonProps> = (props) => {
  return (
    <Button.Root
      type="submit"
      class="transition-color self-start rounded border border-stone-600 bg-orange-950 bg-opacity-80 px-1 py-1
      hover:border-stone-500 hover:bg-orange-900 focus:border-stone-500 focus:outline-none focus:ring focus:ring-stone-700"
    >
      <div class="items center flex">
        <span>{props.text}</span>
        {props.icon}
      </div>
    </Button.Root>
  );
};
