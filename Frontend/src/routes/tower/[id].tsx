import { useParams } from "solid-start";
import { Paper } from "~/components/Paper";
import { RealWorldAtis } from "~/components/RealWorldAtis";
import wretch from "wretch";
import { createResource } from "solid-js";
import { AirportTraffic } from "~/components/AirportTraffic";

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
      <Paper>
        <span>{params.id}</span>
        <RealWorldAtis id={params.id} />
      </Paper>
      <Paper>
        <AirportTraffic faaId={airport()?.faaId} />
      </Paper>
    </>
  );
}
