import { ColumnDef, createSolidTable, flexRender, getCoreRowModel } from "@tanstack/solid-table";
import { Component, For, Show, createResource, onCleanup } from "solid-js";
import wretch from "wretch";
import { Atis, Controller, FlightPlan, Pilot } from "~/vatsimdatafeed";
import { AirlineTelephony } from "./AirlineTelephony";
import { SkyVectorLink } from "./SkyVectorLink";

interface ApiTrafficDto {
  faaId: string;
  onGround: { departures: Pilot[]; arrivals: Pilot[]; noFlightPlan: Pilot[] };
  enrouteArrivals: Pilot[];
  enrouteDepartures: Pilot[];
  controllers: Controller[];
  atis: Atis[];
}

const fetchTrafficForAirport = async (id: string) => {
  const url = new URL(`v1/vatsim/airports/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as ApiTrafficDto;
};

const defaultColumns: ColumnDef<Pilot>[] = [
  {
    accessorKey: "callsign",
    header: "Callsign"
  },
  {
    accessorKey: "callsign",
    header: "Telephony",
    cell: (info) => <AirlineTelephony id={info.getValue<string>().slice(0, 3)} />
  },
  {
    accessorFn: (row) => row.flight_plan?.aircraft_faa,
    id: "type",
    header: "Type",
    cell: (info) => info.getValue<string>()
  },
  {
    accessorFn: (row) => row.flight_plan?.arrival,
    id: "arrival",
    header: "Dest",
    cell: (info) => info.getValue<string>()
  },
  {
    accessorFn: (row) => row.flight_plan?.altitude,
    id: "altitude",
    header: "Altitude",
    cell: (info) => info.getValue<string>()
  },
  {
    accessorFn: (row) => row.flight_plan?.route,
    id: "route",
    header: "Route",
    cell: (info) => info.getValue<string>()
  },
  {
    accessorKey: "cid",
    header: "CID"
  },
  {
    accessorKey: "name",
    header: "Name"
  },
  {
    accessorFn: (row) => row.flight_plan,
    id: "SkyVector",
    header: "SkyVector",
    cell: (info) => (
      <SkyVectorLink
        departure={info.getValue<FlightPlan>().departure}
        arrival={info.getValue<FlightPlan>().arrival}
        route={info.getValue<FlightPlan>().route}
        text="View"
      />
    )
  }
];

interface AirportTrafficProps {
  faaId: string | undefined;
}

export const AirportTraffic: Component<AirportTrafficProps> = (props) => {
  const [traffic, { refetch }] = createResource(() => props.faaId, fetchTrafficForAirport);
  const timer = setInterval(() => refetch(), 20 * 1000);
  onCleanup(() => clearInterval(timer));

  const table = createSolidTable({
    get data() {
      return traffic.state == "ready" ? traffic()?.onGround.departures : [];
    },
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel()
  });

  return (
    <Show when={traffic()}>
      {/* <For each={traffic()?.onGround.departures}>
        {(departure) => <div>{departure.callsign}</div>}
      </For> */}
      <table class="table-auto border-collapse">
        <thead>
          <For each={table.getHeaderGroups()}>
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
          <For each={table.getRowModel().rows}>
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
          <For each={table.getFooterGroups()}>
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
