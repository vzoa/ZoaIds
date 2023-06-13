import {
  createForm,
  maxLength,
  minLength,
  required,
  FormOptions,
  setValue
} from "@modular-forms/solid";
import { Show, createSignal } from "solid-js";
import { useSearchParams } from "solid-start";
import { FlightAwareTable } from "~/components/FlightAwareTable";
import { Paper } from "~/components/Paper";
import { SubmitButton } from "~/components/forms-base/SubmitButton";
import { TextField } from "~/components/forms-base/TextField";

type DepartureArrivalForm = {
  departureId: string;
  arrivalId: string;
};

export default function Reference() {
  const [searchParams] = useSearchParams();

  const [formSubmission, setFormSubmission] = createSignal<DepartureArrivalForm>();
  const options: FormOptions<DepartureArrivalForm> = { revalidateOn: "submit" };
  const [routeForm, { Form, Field }] = createForm<DepartureArrivalForm>(options);
  if (searchParams.departure && searchParams.arrival) {
    setValue(routeForm, "departureId", searchParams.departure);
    setValue(routeForm, "arrivalId", searchParams.arrival);
    setFormSubmission({ departureId: searchParams.departure, arrivalId: searchParams.arrival });
  }

  return (
    <>
      <Paper>
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
        </Form>
      </Paper>
      <Paper title="Real World Routes">
        <Show when={formSubmission()}>
          {(formSubmission) => (
            <FlightAwareTable
              departure={formSubmission().departureId}
              arrival={formSubmission().arrivalId}
            />
          )}
        </Show>
      </Paper>
    </>
  );
}
