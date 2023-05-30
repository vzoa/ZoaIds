import { Component, ErrorBoundary, For, Suspense, createResource, onCleanup } from "solid-js";
import wretch from "wretch";

interface Atis {
  icaoId: string;
  type: string;
  infoLetter: string;
  issueTime: string;
  altimeter: number;
  rawText: string;
  weatherText: string;
  statusText: string;
  uniqueId: string;
}

const fetchAtisForAirport = async (id: string) => {
  const url = new URL(`v1/datis/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as Atis[];
};

interface RealWorldAtisProps {
  id: string;
}

export const RealWorldAtis: Component<RealWorldAtisProps> = (props) => {
  const [atisList, { refetch }] = createResource(() => props.id, fetchAtisForAirport);
  const timer = setInterval(() => refetch(), 60 * 1000);
  onCleanup(() => clearInterval(timer));

  return (
    <ErrorBoundary fallback={<span>Not found</span>}>
      <Suspense>
        <For each={atisList()}>
          {(atis) => (
            <div class="flex items-center">
              <span class="ml-3">{atis.infoLetter}</span>
              <span class="ml-3">{atis.issueTime}</span>
              <span class="ml-3">{atis.weatherText}</span>
              <span class="ml-3">{atis.statusText}</span>
            </div>
          )}
        </For>
      </Suspense>
    </ErrorBoundary>
  );
};
