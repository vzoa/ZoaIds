import { DropdownMenu } from "@kobalte/core";
import { clsx } from "clsx";
import { Component } from "solid-js";
import { A, useLocation, useMatch, useNavigate } from "solid-start";

interface NavBarDropdownItemProps {
  name: string;
  path: string;
}

export const NavBarDropdownItem: Component<NavBarDropdownItemProps> = (props) => {
  const location = useLocation();
  const isActive = useMatch(() => props.path);
  const navigate = useNavigate();
  return (
    <DropdownMenu.Item
      as="li"
      class={clsx(
        "hover whitespace-nowrap px-2 py-1 hover:bg-white hover:bg-opacity-10",
        isActive() && "underline"
      )}
      onSelect={() => navigate(props.path)}
    >
      <A href={props.path} class="focus:outline-none">
        {props.name}
      </A>
    </DropdownMenu.Item>
  );
};
