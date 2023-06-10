import { A, useNavigate } from "solid-start";
import { RouteLookup } from "~/components/RouteLookup";

export default function Reference() {
  const navigate = useNavigate();
  return (
    <div class="m-6 w-1/4 rounded bg-stone-800 p-6 shadow-xl">
      <RouteLookup />
    </div>
  );
}
