import { clsx } from "clsx";
import { ParentComponent, createSignal } from "solid-js";
import { useLocation } from "solid-start";
import { Popover } from "@kobalte/core";
import { useNavBarContext } from "./NavBarContext";

interface NavBarDropdownProps {
  name: string;
  path: string;
}

export const NavBarDropdown: ParentComponent<NavBarDropdownProps> = (props) => {
  const location = useLocation();
  const isActive = () => location.pathname.startsWith(props.path);

  const [open, setOpen] = createSignal(false); // useNavBarContext(); //

  return (
    <li>
      <Popover.Root open={open()} onOpenChange={setOpen}>
        <Popover.Trigger
          class={clsx(
            "m:mx-2 mx-1.5 whitespace-nowrap rounded-md p-2 focus:outline-none focus-visible:ring-2 focus-visible:ring-gray-200",
            isActive() && "bg-orange-950 bg-opacity-80 text-white ring-2 ring-gray-200",
            !isActive() && "hover:bg-orange-500 hover:bg-opacity-30",
            open() && !isActive() && "bg-orange-500 bg-opacity-30"
          )}
        >
          {props.name}
        </Popover.Trigger>
        <Popover.Portal>
          <Popover.Content class="mt-1 max-h-72 overflow-auto rounded-md bg-[#99491f] shadow-xl">
            <div class="h-2" />
            <ul class="list-none">{props.children}</ul>
            <div class="h-2" />
          </Popover.Content>
        </Popover.Portal>
      </Popover.Root>
    </li>
  );
};
export { useNavBarContext };
