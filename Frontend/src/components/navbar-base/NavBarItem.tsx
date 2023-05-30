import { clsx } from "clsx";
import { Component } from "solid-js";
import { A, useLocation } from "solid-start";

interface NavBarItemProps {
  name: string;
  path: string;
}

export const NavBarItem: Component<NavBarItemProps> = (props) => {
  const location = useLocation();
  const isActive = () => props.path == location.pathname;
  return (
    <li>
      <A
        href={props.path}
        class={clsx(
          "mx-1.5 whitespace-nowrap rounded-md p-2 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-gray-200 sm:mx-2",
          isActive() && "bg-orange-950 text-white ring-2 ring-gray-200",
          !isActive() && "hover:bg-orange-500 hover:bg-opacity-30"
        )}
      >
        {props.name}
      </A>
    </li>
  );
};
