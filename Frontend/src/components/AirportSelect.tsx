import { Select } from "@kobalte/core";
import { Component, For, Suspense, createMemo, createResource, on } from "solid-js";
import wretch from "wretch";

interface AirportResponse {
  bravo: string[];
  charlie: string[];
  delta: string[];
}

const fetchAirports = async () => {
  const url = new URL("v1/config/airports", import.meta.env.VITE_IDS_API_BASE).toString();
  const json: AirportResponse = await wretch(url).get().json();
  return json;
};

export const AirportSelect: Component = () => {
  const [airports] = createResource(fetchAirports);
  const airportsList = createMemo(() => {
    if (airports.state == "ready") {
      return airports()?.bravo.concat(
        airports()?.charlie as string[],
        airports()?.delta as string[]
      );
    }
    return [];
  });

  return (
    <Select.Root
      options={airportsList()}
      placeholder="Select an airport"
      itemComponent={(props) => (
        <Select.Item item={props.item}>
          <Select.ItemLabel>{props.item.rawValue}</Select.ItemLabel>
          <Select.ItemIndicator>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="h-5 w-5"
            >
              <path
                fill-rule="evenodd"
                d="M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z"
                clip-rule="evenodd"
              />
            </svg>
          </Select.ItemIndicator>
        </Select.Item>
      )}
    >
      <Select.Trigger
        aria-label="Airport"
        class="inline-flex items-center rounded border border-stone-600 bg-stone-600 p-1 hover:border-stone-500"
      >
        <Select.Value<string>>{(state) => state.selectedOption()}</Select.Value>
        <Select.Icon>
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            class="ml-2 h-5 w-5"
          >
            <path
              fill-rule="evenodd"
              d="M10 3a.75.75 0 01.55.24l3.25 3.5a.75.75 0 11-1.1 1.02L10 4.852 7.3 7.76a.75.75 0 01-1.1-1.02l3.25-3.5A.75.75 0 0110 3zm-3.76 9.2a.75.75 0 011.06.04l2.7 2.908 2.7-2.908a.75.75 0 111.1 1.02l-3.25 3.5a.75.75 0 01-1.1 0l-3.25-3.5a.75.75 0 01.04-1.06z"
              clip-rule="evenodd"
            />
          </svg>
        </Select.Icon>
      </Select.Trigger>
      <Select.Portal>
        <Select.Content class="rounded border border-stone-600 bg-stone-600 p-1 shadow-lg">
          <Select.Listbox />
        </Select.Content>
      </Select.Portal>
    </Select.Root>
  );
};
