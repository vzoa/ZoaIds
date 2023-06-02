import { clsx } from "clsx";
import { ParentComponent, createSignal } from "solid-js";
import { useLocation } from "solid-start";
import { DropdownMenu } from "@kobalte/core";

interface NavBarDropdownProps {
  name: string;
  path: string;
}

export const NavBarDropdown: ParentComponent<NavBarDropdownProps> = (props) => {
  const location = useLocation();
  const isActive = () => location.pathname.startsWith(props.path);
  const [open, setOpen] = createSignal(false);

  return (
    <li>
      <DropdownMenu.Root open={open()} onOpenChange={setOpen}>
        <DropdownMenu.Trigger
          class={clsx(
            "m:mx-2 mx-1.5 whitespace-nowrap rounded-md p-2 transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-gray-200",
            isActive() && "bg-orange-950 bg-opacity-80 text-white ring-2 ring-gray-200",
            !isActive() && "hover:bg-orange-500 hover:bg-opacity-30",
            open() && !isActive() && "bg-orange-500 bg-opacity-30"
          )}
        >
          {props.name}
          <DropdownMenu.Icon>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="inline h-5 w-5"
            >
              <path
                fill-rule="evenodd"
                d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z"
                clip-rule="evenodd"
              />
            </svg>
          </DropdownMenu.Icon>
        </DropdownMenu.Trigger>
        <DropdownMenu.Portal>
          <DropdownMenu.Content class="mt-1 max-h-72 overflow-auto rounded-md bg-[#99491f] shadow-xl scrollbar-thin scrollbar-track-[#99491f] scrollbar-thumb-orange-900 focus:outline-none">
            <div class="h-2" />
            <ul class="list-none">{props.children}</ul>
            <div class="h-2" />
          </DropdownMenu.Content>
        </DropdownMenu.Portal>
      </DropdownMenu.Root>
    </li>
  );
};
