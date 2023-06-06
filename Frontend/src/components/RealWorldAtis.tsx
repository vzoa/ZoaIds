import { Component, ErrorBoundary, For, Suspense, createResource, onCleanup } from "solid-js";
import wretch from "wretch";

interface ApiAtisDto {
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
  return (await wretch(url).get().json()) as ApiAtisDto[];
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
      <Suspense fallback={<span>Loading</span>}>
        <For each={atisList()}>
          {(atis) => (
            <div class="flex items-center">
              <div class="flex flex-col items-center border border-stone-600 p-1.5">
                <span class="text-sm">{atis.type}</span>
                <span class="font-mono text-2xl text-yellow-500">{atis.infoLetter}</span>
                <span class="text-sm">{atis.issueTime.slice(11, 16)}</span>
              </div>
              <span class="ml-3 border border-stone-600 p-1.5 text-sm">{atis.weatherText}</span>
              <span class="ml-3 border border-stone-600 p-1.5 text-sm">{atis.statusText}</span>
            </div>
          )}
        </For>
      </Suspense>
    </ErrorBoundary>
  );
};
