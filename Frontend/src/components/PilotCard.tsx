import { Component, createMemo, mergeProps, Show } from "solid-js";
import { VatsimPilot } from "./interfaces";

interface PilotCardProps {
  pilot: VatsimPilot;
}

export const PilotCard: Component<PilotCardProps> = (props) => {
  return (
    <div class="max-w-sm rounded-xl overflow-hidden shadow-lg">
      <div class="px-6 py-4">
        <div class="mb-2">
          <span class="font-bold text-xl">{props.pilot.callsign}</span>
          <span class="text-lg ml-1 text-gray-500">({props.pilot.flight_plan.aircraft_faa})</span>
        </div>
        <Show when={props.pilot.flight_plan} fallback={<span class="italic">No flight plan</span>}>
          <div class="mb-2">
            {props.pilot.flight_plan.departure}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="w-5 h-5 inline mx-1"
            >
              <path
                fill-rule="evenodd"
                d="M2 10a.75.75 0 01.75-.75h12.59l-2.1-1.95a.75.75 0 111.02-1.1l3.5 3.25a.75.75 0 010 1.1l-3.5 3.25a.75.75 0 11-1.02-1.1l2.1-1.95H2.75A.75.75 0 012 10z"
                clip-rule="evenodd"
              />
            </svg>
            {props.pilot.flight_plan.arrival}
          </div>
        </Show>
        <p class="text-gray-700 text-base">{props.pilot.flight_plan.route}</p>
      </div>
    </div>
  );
};
