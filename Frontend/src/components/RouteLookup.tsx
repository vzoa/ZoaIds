import { createForm, maxLength, minLength, required, FormOptions } from "@modular-forms/solid";
import { Component, createResource, createSignal } from "solid-js";
import { TextField } from "./forms-base/TextField";
import wretch from "wretch";
import { SubmitButton } from "./forms-base/SubmitButton";

type DepartureArrivalForm = {
  departureId: string;
  arrivalId: string;
};

interface AirportPairRouteSummary {
  departureIcaoId: string;
  arrivalIcaoId: string;
  flightRouteSummary: FlightRouteSummary[];
  mostRecent: Flight[];
}

interface FlightRouteSummary {
  departureIcaoId: string;
  arrivalIcaoId: string;
  routeFrequency: number;
  minAltitude?: number;
  maxAltitude?: number;
  route: string;
  distanceMi?: string;
  flights: Flight[];
}

interface Flight {
  departureIcaoId: string;
  arrivalIcaoId: string;
  callsign: string;
  aircraftIcaoId: string;
  altitude?: number;
  route: string;
  distance?: number;
}

const fetchChartsForAirport = async (props: DepartureArrivalForm) => {
  const url = new URL(
    `v1/routes/${props.departureId}/${props.arrivalId}`,
    import.meta.env.VITE_IDS_API_BASE
  ).toString();
  const json: AirportPairRouteSummary = await wretch(url).get().json();
  return json;
};

export const RouteLookup: Component = () => {
  const [formSubmission, setFormSubmission] = createSignal<DepartureArrivalForm>();
  const [fetchedRoutes] = createResource(formSubmission, fetchChartsForAirport);
  const options: FormOptions<DepartureArrivalForm> = { revalidateOn: "submit" };
  const [routeForm, { Form, Field }] = createForm<DepartureArrivalForm>(options);
  return (
    <Form onSubmit={(values) => setFormSubmission(values)}>
      <div class="flex gap-7">
        <Field
          name="departureId"
          validate={[
            required("Departure Airport ID required"),
            minLength(3, "3 or 4 characters only"),
            maxLength(4, "3 or 4 characters only")
          ]}
        >
          {(field, props) => (
            <TextField
              {...props}
              type="text"
              label="Departure:"
              placeholder="ICAO"
              value={field.value}
              error={field.error}
              required
            />
          )}
        </Field>
        <Field
          name="arrivalId"
          validate={[
            required("Arrival Airport ID required"),
            minLength(3, "3 or 4 characters only"),
            maxLength(4, "3 or 4 characters only")
          ]}
        >
          {(field, props) => (
            <TextField
              {...props}
              type="text"
              label="Arrival:"
              placeholder="ICAO"
              value={field.value}
              error={field.error}
              required
            />
          )}
        </Field>
        <SubmitButton
          text="Search"
          icon={
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke-width="1.5"
              stroke="currentColor"
              class="ml-1 h-6 w-6"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M15.75 15.75l-2.489-2.489m0 0a3.375 3.375 0 10-4.773-4.773 3.375 3.375 0 004.774 4.774zM21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          }
        />
      </div>

      <div>{JSON.stringify(fetchedRoutes())}</div>
    </Form>

    // <Form
    //   onSubmit={(values) => {
    //     setDepartureId(values.departureId);
    //     setArrivalId(values.arrivalId);
    //   }}
    // >
    //   <Field name="departureId">{(field, props) => <input {...props} />}</Field>
    //   <Field name="arrivalId">{(field, props) => <input {...props} />}</Field>
    // </Form>
  );
};
