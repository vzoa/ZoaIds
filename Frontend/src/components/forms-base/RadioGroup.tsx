import { RadioGroup as Kobalte } from "@kobalte/core";
import { type JSX, Show, splitProps, For } from "solid-js";

type RadioGroupProps = {
  name: string;
  label?: string | undefined;
  options: { label: string; value: string }[];
  value: string | undefined;
  error: string;
  required?: boolean | undefined;
  disabled?: boolean | undefined;
  ref: (element: HTMLInputElement | HTMLTextAreaElement) => void;
  onInput: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, InputEvent>;
  onChange: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, Event>;
  onBlur: JSX.EventHandler<HTMLInputElement | HTMLTextAreaElement, FocusEvent>;
};

export function RadioGroup(props: RadioGroupProps) {
  const [rootProps, inputProps] = splitProps(
    props,
    ["name", "value", "required", "disabled"],
    ["ref", "onInput", "onChange", "onBlur"]
  );
  return (
    <Kobalte.Root {...rootProps} validationState={props.error ? "invalid" : "valid"}>
      <Show when={props.label}>
        <Kobalte.Label>{props.label}</Kobalte.Label>
      </Show>
      <div>
        <For each={props.options}>
          {(option) => (
            <Kobalte.Item value={option.value}>
              <Kobalte.ItemInput {...inputProps} />
              <Kobalte.ItemControl>
                <Kobalte.ItemIndicator />
              </Kobalte.ItemControl>
              <Kobalte.ItemLabel>{option.label}</Kobalte.ItemLabel>
            </Kobalte.Item>
          )}
        </For>
      </div>
      <Kobalte.ErrorMessage>{props.error}</Kobalte.ErrorMessage>
    </Kobalte.Root>
  );
}
