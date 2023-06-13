import { Table, flexRender } from "@tanstack/solid-table";
import { For, JSX } from "solid-js";

interface TableRenderProps<T> {
  table: Table<T>;
}

export function TableRender<T>(props: TableRenderProps<T>): JSX.Element {
  return (
    <table class="mx-0.5 table-auto border-collapse text-sm">
      <thead>
        <For each={props.table.getHeaderGroups()}>
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
        <For each={props.table.getRowModel().rows}>
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
        <For each={props.table.getFooterGroups()}>
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
  );
}
