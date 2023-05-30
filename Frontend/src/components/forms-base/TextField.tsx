import { TextField as Kobalte } from "@kobalte/core";
import { type JSX, Show, splitProps } from "solid-js";

type TextFieldProps = {
  name: string;
  type?: "text" | "email" | "tel" | "password" | "url" | "date" | undefined;
  label?: string | undefined;
  placeholder?: string | undefined;
  value: string | undefined;
  error: string;
  multiline?: boolean | undefined;
  required?: boolean | undefined;
  disabled?: boolean | undefined;
  rootClass?: string | undefined;
  labelClass?: string | undefined;
  ref: (element: HTMLInputElement | HTMLTextAreaElement) => void;
  onInput: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, InputEvent>;
  onChange: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, Event>;
  onBlur: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, FocusEvent>;
};

export function TextField(props: TextFieldProps) {
  const [rootProps, inputProps] = splitProps(
    props,
    ["name", "value", "required", "disabled"],
    ["placeholder", "ref", "onInput", "onChange", "onBlur"]
  );
  return (
    <Kobalte.Root
      {...rootProps}
      validationState={props.error ? "invalid" : "valid"}
      class="flex items-center"
    >
      <Show when={props.label}>
        <Kobalte.Label>{props.label}</Kobalte.Label>
      </Show>
      <Show
        when={props.multiline}
        fallback={
          <Kobalte.Input
            class="ml-2 w-16 rounded border border-stone-600 bg-transparent p-1 hover:border-stone-500 focus:border-stone-500 focus:outline-none focus:ring focus:ring-stone-700"
            {...inputProps}
            type={props.type}
          />
        }
      >
        <Kobalte.TextArea {...inputProps} autoResize />
      </Show>
      <Kobalte.ErrorMessage>{props.error}</Kobalte.ErrorMessage>
    </Kobalte.Root>
  );
}
