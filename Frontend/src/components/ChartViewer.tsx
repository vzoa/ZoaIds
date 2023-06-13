import { FormOptions, createForm, custom } from "@modular-forms/solid";
import { Component, For, Setter, Show, createMemo, createResource, createSignal } from "solid-js";
import wretch from "wretch";
import { SubmitButton } from "./forms-base/SubmitButton";
import { TextField } from "./forms-base/TextField";
import { HiOutlineMagnifyingGlassCircle } from "solid-icons/hi";
import {
  ColumnDef,
  RowData,
  createSolidTable,
  flexRender,
  getCoreRowModel
} from "@tanstack/solid-table";
import { Button } from "@kobalte/core";

type ChartViewerProps =
  | {
      includeForm: false | undefined;
      id?: string;
      search: string;
    }
  | {
      includeForm: true;
      id?: string;
    };

type ChartSearchForm = {
  search: string;
};

interface ChartsRoot {
  airportName: string;
  faaIdent: string;
  icaoIdent: string;
  charts: Chart[];
}

interface Chart {
  chartSeq: string;
  chartCode: string;
  chartName: string;
  pages: ChartPage[];
}

interface ChartPage {
  pageNumber: number;
  pdfName: string;
  pdfPath: string;
}

declare module "@tanstack/table-core" {
  interface TableMeta<TData extends RowData> {
    displayedChartSetter: Setter<string>;
  }
}

const fetchChartsForAirport = async (id: string) => {
  const url = new URL(`v1/charts/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  const json: ChartsRoot = await wretch(url).get().json();
  return json.charts;
};

const submitWithId = (search: string, idSetter: Setter<string>, searchSetter: Setter<string>) => {
  let splits = search.split(" ", 2);
  idSetter(splits[0]);
  if (splits.length == 2) {
    searchSetter(splits[1]);
  }
};

const defaultColumns: ColumnDef<Chart>[] = [
  {
    accessorKey: "chartCode",
    header: "Type"
  },
  {
    accessorKey: "chartName",
    header: "Name"
  },
  {
    accessorKey: "pages",
    header: "Pages",
    cell: (info) => {
      return (
        <div class="flex gap-2">
          <For each={info.getValue<ChartPage[]>()}>
            {(page) => (
              <button
                class="hover:underline"
                onClick={() => info.table.options.meta!.displayedChartSetter(page.pdfPath)}
              >
                {page.pageNumber}
              </button>
            )}
          </For>
        </div>
      );
    }
  }
];

export const ChartViewer: Component<ChartViewerProps> = (props) => {
  const [searchId, setSearchId] = createSignal(() => props.id || "");
  const [searchString, setSearchString] = createSignal(!props.includeForm ? props.search : "");
  const [displayedChartUrl, setDisplayedChartUrl] = createSignal("");

  const [charts] = createResource(searchId(), fetchChartsForAirport);

  const matchedCharts = createMemo(() =>
    charts()?.filter((chart) =>
      chart.chartName.toUpperCase().includes(searchString().toUpperCase())
    )
  );

  const table = createSolidTable({
    get data() {
      return matchedCharts() ?? [];
    },
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel(),
    meta: { displayedChartSetter: setDisplayedChartUrl }
  });

  const options: FormOptions<ChartSearchForm> = { revalidateOn: "submit" };
  const [chartForm, { Form, Field }] = createForm<ChartSearchForm>(options);

  return (
    // <For each={matchedCharts()}>
    //   {(chart) => (
    //     <object type="application/pdf" width="100%" height="1050px" data={chart.pages[0].pdfPath} />
    //   )}
    // </For>
    <>
      <div>
        <Show when={props.includeForm}>
          <Form
            onSubmit={(values) => {
              if (props.id) setSearchString(values.search);
              else submitWithId(values.search, setSearchId, setSearchString);
              setDisplayedChartUrl("");
            }}
          >
            <div class="flex gap-7">
              <Field
                name="search"
                validate={[
                  custom(
                    (value) => (value ? (searchId() ? true : value.length > 0) : false),
                    "Search string required"
                  )
                ]}
              >
                {(field, props) => (
                  <TextField
                    {...props}
                    type="text"
                    label="Search:"
                    placeholder=""
                    value={field.value}
                    error={field.error}
                    inputClass="w-48"
                    required
                  />
                )}
              </Field>
              <SubmitButton
                text="Search"
                icon={<HiOutlineMagnifyingGlassCircle size={24} class="ml-1" />}
              />
            </div>
          </Form>
        </Show>
      </div>
      <div>
        <table class="mx-0.5 table-auto border-collapse text-sm">
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
      </div>
      <Show when={displayedChartUrl()}>
        {(url) => (
          <object class="mt-2" type="application/pdf" width="100%" height="1050px" data={url()} />
        )}
      </Show>
    </>
  );

  // const groupedCharts = () => {
  //   const acc: Record<string, Chart[]> = {};
  //   if (charts() != undefined) {
  //     for (const chart of charts()!) {
  //       if (!(chart["chart_code"] in acc)) {
  //         acc[chart["chart_code"]] = [chart];
  //       } else {
  //         acc[chart["chart_code"]].push(chart);
  //       }
  //     }
  //   }
  //   return acc;
  // };

  // createEffect(() => {
  //   console.log(JSON.stringify(groupedCharts()));
  // });

  //return <Suspense fallback={<p>"Loading"</p>}>{JSON.stringify(charts(), null, 2)}</Suspense>;
};
