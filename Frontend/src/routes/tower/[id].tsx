import { useParams } from "solid-start";
import { Paper } from "~/components/Paper";
import { RealWorldAtis } from "~/components/RealWorldAtis";

export default function AirportPage() {
  const params = useParams<{ id: string }>();
  return (
    <Paper>
      <span>{params.id}</span>
      <RealWorldAtis id={params.id} />
    </Paper>
  );
}
