import { useParams } from "solid-start";
import { RealWorldAtis } from "~/components/RealWorldAtis";
import wretch from "wretch";
import { Show, createResource } from "solid-js";
import { AirportTraffic } from "~/components/AirportTraffic";
import { CollapsiblePaper } from "~/components/CollapsiblePaper";
import { VatsimAtis } from "~/components/VatsimAtis";
import { ChartViewer } from "~/components/ChartViewer";

interface Airport {
  faaId: string;
  icaoId: string;
  name: string;
  type: string;
  trueToMagneticDelta: number;
  magneticToTrueDelta: number;
  location: { latitude: number; longitude: number };
  elevation: number;
  artcc: string;
  runways: Runway[];
  runwayEnds: End[];
}

interface Runway {
  name: string;
  length: number;
  ends: End[];
  airportFaaId: string;
}

interface End {
  name: string;
  trueHeading: number;
  magneticHeading: number;
  endElevation: number;
  tdzElevation: number;
  runwayName: string;
  airportFaaId: string;
}

const fetchAirportData = async (id: string) => {
  const url = new URL(`v1/airports/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as Airport;
};

export default function AirportPage() {
  const params = useParams<{ id: string }>();
  const [airport] = createResource(() => params.id, fetchAirportData);
  return (
    <>
      <CollapsiblePaper title="Real World D-Atis">
        <RealWorldAtis id={params.id} />
      </CollapsiblePaper>
      <CollapsiblePaper title="Vatsim D-Atis">
        <VatsimAtis id={params.id} />
      </CollapsiblePaper>
      <CollapsiblePaper defaultOpen title="Traffic Situation">
        <Show when={airport()}>{(airport) => <AirportTraffic faaId={airport().faaId} />}</Show>
      </CollapsiblePaper>
      <CollapsiblePaper defaultOpen title="Charts">
        <ChartViewer includeForm id={params.id} />
      </CollapsiblePaper>
    </>
  );
}
