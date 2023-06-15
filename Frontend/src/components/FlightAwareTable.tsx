import { ColumnDef, createSolidTable, flexRender, getCoreRowModel } from "@tanstack/solid-table";
import { Component, For, Setter, Show, createResource } from "solid-js";
import wretch from "wretch";
import { Spinner } from "./Spinner";
import { writeClipboard } from "@solid-primitives/clipboard";
import { HiOutlineClipboard } from "solid-icons/hi";

export interface FlightAwareRouteApiDto {
  departureIcaoId: string;
  arrivalIcaoId: string;
  flightRouteSummaries: FlightRouteSummary[];
  mostRecent: Flight[];
}

export interface FlightRouteSummary {
  departureIcaoId: string;
  arrivalIcaoId: string;
  routeFrequency: number;
  minAltitude: number;
  maxAltitude: number;
  route: string;
  distanceMi: number;
  flights: Flight[];
}

export interface Flight {
  departureIcaoId: string;
  arrivalIcaoId: string;
  callsign: string;
  aircraftIcaoId: string;
  altitude: number;
  route: string;
  distance: number;
}

const fetchFlightAwareRoute = async (props: { departure: string; arrival: string }) => {
  const url = new URL(
    `v1/routes/${props.departure}/${props.arrival}`,
    import.meta.env.VITE_IDS_API_BASE
  ).toString();
  return (await wretch(url).get().json()) as FlightAwareRouteApiDto;
};

const summaryColumns: ColumnDef<FlightRouteSummary>[] = [
  {
    accessorKey: "routeFrequency",
    header: "Frequency"
  },
  {
    accessorKey: "minAltitude",
    header: "Min Alt"
  },
  {
    accessorKey: "maxAltitude",
    header: "Max Alt"
  },
  {
    accessorKey: "route",
    header: "Route",
    cell: (info) => {
      return (
        <>
          <HiOutlineClipboard
            class="mr-2 inline cursor-pointer"
            onClick={() => writeClipboard(info.getValue<string>())}
          />
          <span>{info.getValue<string>()}</span>
        </>
      );
    }
  }
];

const recentColumns: ColumnDef<Flight>[] = [
  {
    accessorKey: "callsign",
    header: "Callsign"
  },
  {
    accessorKey: "aircraftIcaoId",
    header: "Type"
  },
  {
    accessorKey: "altitude",
    header: "Altitude"
  },
  {
    accessorKey: "route",
    header: "Route",
    cell: (info) => {
      return (
        <>
          <HiOutlineClipboard
            class="mr-2 inline cursor-pointer"
            onClick={() => writeClipboard(info.getValue<string>())}
          />
          <span>{info.getValue<string>()}</span>
        </>
      );
    }
  }
];

interface FlightAwareTableProps {
  departure: string;
  arrival: string;
}

export const FlightAwareTable: Component<FlightAwareTableProps> = (props) => {
  const [flights] = createResource(() => ({ ...props }), fetchFlightAwareRoute);

  const summaryTable = createSolidTable({
    get data() {
      return flights.state == "ready" ? flights().flightRouteSummaries : [];
    },
    columns: summaryColumns,
    getCoreRowModel: getCoreRowModel()
  });

  const recentTable = createSolidTable({
    get data() {
      return flights.state == "ready" ? flights().mostRecent : [];
    },
    columns: recentColumns,
    getCoreRowModel: getCoreRowModel()
  });

  return (
    <Show when={flights.state == "ready"} fallback={<Spinner />}>
      {/* <h1 class="text-xl">Routes</h1> */}
      <table class="table-auto border-collapse text-sm">
        <thead>
          <For each={summaryTable.getHeaderGroups()}>
            {(headerGroup) => (
              <tr>
                <For each={headerGroup.headers}>
                  {(header) => (
                    <th class="p-2 text-left">
                      {header.isPlaceholder
                        ? null
                        : flexRender(header.column.columnDef.header, header.getContext())}
                    </th>
                  )}
                </For>
              </tr>
            )}
          </For>
        </thead>
        <tbody>
          <For each={summaryTable.getRowModel().rows}>
            {(row) => (
              <tr class="font-mono transition-colors hover:bg-stone-700">
                <For each={row.getVisibleCells()}>
                  {(cell) => (
                    <td class="border border-stone-600 px-2 py-0.5">
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  )}
                </For>
              </tr>
            )}
          </For>
        </tbody>
        <tfoot>
          <For each={summaryTable.getFooterGroups()}>
            {(footerGroup) => (
              <tr>
                <For each={footerGroup.headers}>
                  {(header) => (
                    <th>
                      {header.isPlaceholder
                        ? null
                        : flexRender(header.column.columnDef.footer, header.getContext())}
                    </th>
                  )}
                </For>
              </tr>
            )}
          </For>
        </tfoot>
      </table>

      <h1 class="mt-4 text-xl">Most Recent</h1>
      <table class="table-auto border-collapse text-sm">
        <thead>
          <For each={recentTable.getHeaderGroups()}>
            {(headerGroup) => (
              <tr>
                <For each={headerGroup.headers}>
                  {(header) => (
                    <th class="p-2 text-left">
                      {header.isPlaceholder
                        ? null
                        : flexRender(header.column.columnDef.header, header.getContext())}
                    </th>
                  )}
                </For>
              </tr>
            )}
          </For>
        </thead>
        <tbody>
          <For each={recentTable.getRowModel().rows}>
            {(row) => (
              <tr class="font-mono transition-colors hover:bg-stone-700">
                <For each={row.getVisibleCells()}>
                  {(cell) => (
                    <td class="border border-stone-600 px-2 py-0.5">
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  )}
                </For>
              </tr>
            )}
          </For>
        </tbody>
        <tfoot>
          <For each={recentTable.getFooterGroups()}>
            {(footerGroup) => (
              <tr>
                <For each={footerGroup.headers}>
                  {(header) => (
                    <th>
                      {header.isPlaceholder
                        ? null
                        : flexRender(header.column.columnDef.footer, header.getContext())}
                    </th>
                  )}
                </For>
              </tr>
            )}
          </For>
        </tfoot>
      </table>
    </Show>
  );
};
