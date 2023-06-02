import { Component, For, Show, Suspense, createMemo, createResource } from "solid-js";
import { TimeDisplay } from "./TimeDisplay";
import { NavBarDropdownItem, NavBarDropdown, NavBarItem } from "./navbar-base";
import wretch from "wretch";
import { ErrorBoundary } from "solid-start";

interface AirportResponse {
  bravo: string[];
  charlie: string[];
  delta: string[];
}

const fetchAirports = async () => {
  const url = new URL("v1/config/airports", import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as AirportResponse;
};

export const NavBar: Component = () => {
  const [airports] = createResource(fetchAirports);
  const airportsList = createMemo(() => {
    if (airports.state == "ready") {
      return airports().bravo.concat(airports().charlie as string[], airports().delta as string[]);
    }
    return [];
  });

  return (
    <div class="flex items-center bg-orange-900 py-1">
      <TimeDisplay />
      <nav>
        <ul class="flex list-none items-center p-1 text-gray-200">
          <NavBarItem name="Home" path="/" />
          <NavBarItem name="ZOA Summary" path="/summary" />
          <NavBarDropdown name="Tower" path="/tower">
            <ErrorBoundary>
              <Show when={airports()}>
                <For each={airportsList()}>
                  {(airport) => (
                    <NavBarDropdownItem name={airport} path={`/tower/${airport.toLowerCase()}`} />
                  )}
                </For>
              </Show>
            </ErrorBoundary>
          </NavBarDropdown>
          <NavBarItem name="TRACON" path="/about" />
          <NavBarItem name="Reference" path="/reference" />
          <NavBarItem name="Pireps" path="/about" />
          <NavBarDropdown name="Dropdown" path="/dropdown">
            <NavBarDropdownItem name="Dropdown" path="/dropdown" />
            <NavBarDropdownItem name="Dropdown" path="/dropdown" />
            <NavBarDropdownItem name="Dropdown" path="/dropdown" />
            <NavBarDropdownItem name="Dropdown" path="/dropdown" />
            <NavBarDropdownItem name="Dropdown" path="/dropdown" />
          </NavBarDropdown>
        </ul>
      </nav>
    </div>
  );
};
