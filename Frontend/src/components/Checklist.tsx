import { Checkbox } from "@kobalte/core";
import { Component, Index, createSignal } from "solid-js";

export const Checklist: Component<{ items: string[] }> = (props) => {
  return (
    <Index each={props.items}>
      {(item) => {
        const [checked, setChecked] = createSignal(false);
        const labelClass = () => (checked() ? "line-through text-gray-400" : "");
        return (
          <>
            <Checkbox.Root checked={checked()} onChange={setChecked} class="flex items-center">
              <Checkbox.Input />
              <Checkbox.Control class="w-5 h-5 mr-2 text-blue-600 bg-gray-100 border-gray-300 border border-opacity-75 rounded focus:ring-blue-500 hover:border-gray-400">
                <Checkbox.Indicator>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                    class="w-5 h-5"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z"
                      clip-rule="evenodd"
                    />
                  </svg>
                </Checkbox.Indicator>
              </Checkbox.Control>
              <Checkbox.Label class={labelClass()}>{item()}</Checkbox.Label>
            </Checkbox.Root>
          </>
        );
      }}
    </Index>
  );
};

// <div class="block">
// <div class="mt-2">
//   <label class="inline-flex items-center">
//     {/* <input type="checkbox" class="w-6 h-6 rounded" /> */}
//     <div
//       class="w-6 h-6 border-2 rounded bg-slate-500 border-cyan-400 hover:bg-slate-300"
//       onClick={() => setChecked((prev) => !prev)}
//     />
//     <span class={labelClass()}>{item()}</span>
//   </label>
// </div>
// </div>
