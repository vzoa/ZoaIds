import { ColumnDef, createSolidTable, flexRender, getCoreRowModel } from "@tanstack/solid-table";
import { Component, For, Show } from "solid-js";
import wretch from "wretch";
import { Atis, Controller, FlightPlan, Pilot } from "~/vatsimdatafeed";
import { AirlineTelephony } from "./AirlineTelephony";
import { SkyVectorLink } from "./SkyVectorLink";
import { FlightAwareModal } from "./FlightAwareModal";
import { createQuery } from "@tanstack/solid-query";

interface ApiTrafficDto {
  faaId: string;
  update: string;
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
    id: "skyvector",
    header: "SkyVector",
    cell: (info) => (
      <SkyVectorLink
        departure={info.getValue<FlightPlan>().departure}
        arrival={info.getValue<FlightPlan>().arrival}
        route={info.getValue<FlightPlan>().route}
        text="View"
      />
    )
  },
  {
    accessorFn: (row) => row.flight_plan,
    id: "realroute",
    header: "Real Route",
    cell: (info) => (
      <FlightAwareModal
        departure={info.getValue<FlightPlan>().departure}
        arrival={info.getValue<FlightPlan>().arrival}
        text="View"
      />
    )
  },
  {
    accessorKey: "last_updated",
    header: "Last Updated"
  }
];

interface AirportTrafficProps {
  faaId: string;
}

export const AirportTraffic: Component<AirportTrafficProps> = (props) => {
  // const [traffic, { refetch }] = createResource(() => props.faaId, fetchTrafficForAirport, {
  //   storage: createDeepSignal
  // });

  const query = createQuery(
    () => ["trafficId", props.faaId],
    () => fetchTrafficForAirport(props.faaId),
    { refetchInterval: 20 * 1000 }
  );

  //const tableData = () => traffic.latest?.onGround.departures ?? [];

  //const timer = setInterval(() => refetch(), 20 * 1000);
  //onCleanup(() => clearInterval(timer));

  const table = createSolidTable({
    get data() {
      return query.data?.onGround.departures ?? [];
      //return tableData();
    },
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel()
  });

  return (
    <Show when={query.data}>
      <table class="table-auto border-collapse text-sm">
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
