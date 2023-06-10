import {
  Component,
  ErrorBoundary,
  For,
  Suspense,
  createMemo,
  createResource,
  onCleanup
} from "solid-js";
import wretch from "wretch";
import { Atis } from "~/vatsimdatafeed";

const fetchAtisForAirport = async (id: string) => {
  const url = new URL(`v1/vatsim/datafeed/atis`, import.meta.env.VITE_IDS_API_BASE).toString();
  let allAtis: Atis[] = await wretch(url).get().json();
  return allAtis.filter((atis) => atis.callsign.startsWith(id.toUpperCase()));
};

interface ParsedAtis {
  icaoId: string;
  type: "Departure" | "Arrival" | "Combined";
  infoLetter?: string;
  issueTime: string;
  altimeter?: number;
  rawText: string;
  weatherText: string;
  statusText: string;
}

const parseAtis = (atis: Atis) => {
  let rawText = atis.text_atis?.join(" ") ?? "";
  let issueTimeMatch = rawText.match("[0-9]{4}Z");
  let altimeterMatch = rawText.match("A([0-9]{4})");

  let parsed: ParsedAtis = {
    icaoId: atis.callsign.slice(0, 4),
    type: atis.callsign.includes("_A_")
      ? "Arrival"
      : atis.callsign.includes("_D_")
      ? "Departure"
      : "Combined",
    infoLetter: atis.atis_code,
    rawText: rawText,
    issueTime: issueTimeMatch ? issueTimeMatch[0] : "",
    altimeter: altimeterMatch ? parseInt(altimeterMatch[1], 10) : undefined,
    weatherText: rawText.split(". ")[1],
    statusText: rawText.split(". ").slice(2).join(". ")
  };
  return parsed;
};

interface VatsimAtisProps {
  id: string;
}

export const VatsimAtis: Component<VatsimAtisProps> = (props) => {
  const [atisList, { refetch }] = createResource(() => props.id, fetchAtisForAirport);
  const parsedAtisList = createMemo(() => atisList()?.map((a) => parseAtis(a)));
  const timer = setInterval(() => refetch(), 60 * 1000);
  onCleanup(() => clearInterval(timer));

  return (
    <ErrorBoundary fallback={<span>Not found</span>}>
      <Suspense fallback={<span>Loading</span>}>
        <For each={parsedAtisList()}>
          {(atis) => (
            <div class="flex justify-stretch">
              <div class="flex items-center border border-stone-600 p-1.5 text-sm">
                <div class="flex flex-col items-center">
                  <span class="text-sm">{atis.type}</span>
                  <span class="font-mono text-2xl text-yellow-500">{atis.infoLetter}</span>
                  <span class="text-sm">
                    {atis.issueTime.slice(0, 2)}:{atis.issueTime.slice(2, 4)}
                  </span>
                </div>
              </div>
              <div class="ml-3 flex items-center border border-stone-600 p-1.5 text-sm">
                <span>{atis.weatherText}</span>
              </div>
              <div class="ml-3 flex items-center border border-stone-600 p-1.5 text-sm">
                <span>{atis.statusText}</span>
              </div>
            </div>
          )}
        </For>
      </Suspense>
    </ErrorBoundary>
  );
};
