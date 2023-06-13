import { DropdownMenu } from "@kobalte/core";
import { clsx } from "clsx";
import { Component } from "solid-js";
import { A, useLocation, useMatch, useNavigate } from "solid-start";
import { useNavContext } from "../NavContext";

interface NavBarDropdownItemProps {
  name: string;
  path: string;
}

export const NavBarDropdownItem: Component<NavBarDropdownItemProps> = (props) => {
  const location = useLocation();
  const isActive = useMatch(() => props.path);
  const navigate = useNavigate();
  const [navBackState, { clearNavBack }] = useNavContext();
  return (
    <DropdownMenu.Item
      as="li"
      class={clsx(
        "hover whitespace-nowrap hover:bg-white hover:bg-opacity-10",
        isActive() && "underline"
      )}
      onSelect={() => {
        navigate(props.path);
        clearNavBack();
      }}
    >
      <A href={props.path} class="inline-block h-max w-max px-2 py-1 focus:outline-none">
        {props.name}
      </A>
    </DropdownMenu.Item>
  );
};
