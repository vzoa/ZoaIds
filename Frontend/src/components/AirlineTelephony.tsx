import { Component, ErrorBoundary, For, Suspense, createResource, onCleanup } from "solid-js";
import wretch from "wretch";

export interface AirlineDto {
  icaoId: string;
  callsign: string;
  name: string;
  country: string;
}

const fetchInfoForAirline = async (id: string) => {
  const url = new URL(`v1/airlines/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as AirlineDto;
};

interface AirlineTelephony {
  id: string;
  class?: string;
}

export const AirlineTelephony: Component<AirlineTelephony> = (props) => {
  const [airline] = createResource(() => props.id, fetchInfoForAirline);

  return (
    <ErrorBoundary fallback={<span class={props.class}>Not found</span>}>
      <Suspense fallback={<span class={props.class}></span>}>
        <span class={props.class}>{airline()?.callsign || "None"}</span>
      </Suspense>
    </ErrorBoundary>
  );
};
