import { Component, For, Show, createMemo, createResource } from "solid-js";
import { TimeDisplay } from "./TimeDisplay";
import { NavBarDropdownItem, NavBarDropdown, NavBarItem } from "./navbar-base";
import wretch from "wretch";
import { A, ErrorBoundary, useNavigate } from "solid-start";
import { HiOutlineChevronLeft } from "solid-icons/hi";
import { useNavContext } from "./NavContext";

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
  const [navBackState, { clearNavBack }] = useNavContext();

  return (
    <>
      <div class="sticky left-0 top-0 z-40">
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
                        <NavBarDropdownItem
                          name={airport}
                          path={`/tower/${airport.toLowerCase()}`}
                        />
                      )}
                    </For>
                  </Show>
                </ErrorBoundary>
              </NavBarDropdown>
              <NavBarItem name="TRACON" path="/tracon" />
              <NavBarDropdown name="Reference" path="/reference">
                <NavBarDropdownItem name="Routes" path="/reference/routes" />
              </NavBarDropdown>
              <NavBarItem name="Pireps" path="/pireps" />
            </ul>
          </nav>
        </div>
        <Show when={navBackState.show}>
          <div class="bg-orange-950 py-1">
            <div class="ml-3">
              <A href={navBackState.path} class="flex items-center" onClick={() => clearNavBack()}>
                <HiOutlineChevronLeft />
                <span class="ml-1">Back to {navBackState.text}</span>
              </A>
            </div>
          </div>
        </Show>
      </div>
    </>
  );
};
