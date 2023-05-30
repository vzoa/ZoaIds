import { Checkbox as Kobalte } from "@kobalte/core";
import { type JSX, splitProps } from "solid-js";

type CheckboxProps = {
  name: string;
  label: string;
  value?: string | undefined;
  checked: boolean | undefined;
  error: string;
  required?: boolean | undefined;
  disabled?: boolean | undefined;
  ref: (element: HTMLInputElement) => void;
  onInput: JSX.EventHandler<HTMLInputElement, InputEvent>;
  onChange: JSX.EventHandler<HTMLInputElement, Event>;
  onBlur: JSX.EventHandler<HTMLInputElement, FocusEvent>;
};

export function Checkbox(props: CheckboxProps) {
  const [rootProps, inputProps] = splitProps(
    props,
    ["name", "value", "checked", "required", "disabled"],
    ["ref", "onInput", "onChange", "onBlur"]
  );
  return (
    <Kobalte.Root {...rootProps} validationState={props.error ? "invalid" : "valid"}>
      <Kobalte.Input {...inputProps} />
      <Kobalte.Control>
        <Kobalte.Indicator>{/* Add SVG icon here */}</Kobalte.Indicator>
      </Kobalte.Control>
      <Kobalte.Label>{props.label}</Kobalte.Label>
    </Kobalte.Root>
  );
}
