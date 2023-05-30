import { clsx } from "clsx";
import { Component, Setter } from "solid-js";
import { A } from "solid-start";

interface NavBarDropdownItemProps {
  name: string;
  path: string;
  setOpen?: Setter<boolean>;
}

export const NavBarDropdownItem: Component<NavBarDropdownItemProps> = (props) => {
  return (
    <li class={clsx("hover whitespace-nowrap px-2 py-1 hover:bg-white hover:bg-opacity-10")}>
      <A href={props.path} class="focus:outline-none">
        {props.name}
      </A>
    </li>
  );
};
