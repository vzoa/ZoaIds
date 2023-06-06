import { Dialog } from "@kobalte/core";
import { Component, splitProps } from "solid-js";
import { A } from "solid-start";
import { FaSolidXmark } from "solid-icons/fa";
import { FlightAwareTable } from "./FlightAwareTable";

interface FlightAwareModalProps {
  departure: string;
  arrival: string;
  text: string;
}

export const FlightAwareModal: Component<FlightAwareModalProps> = (props) => {
  const [local, modalProps] = splitProps(props, ["text"]);
  return (
    <Dialog.Root>
      <Dialog.Trigger as="a">{local.text}</Dialog.Trigger>
      <Dialog.Portal>
        <Dialog.Overlay class="fixed inset-0 bg-white bg-opacity-40 duration-300 ease-in-out" />
        <div class="z-50 flex items-center justify-center">
          <Dialog.Content class="z-50 rounded border border-stone-600 bg-stone-800 p-2 shadow-xl">
            <div class="mb-3 flex justify-between">
              <Dialog.Title class="text-xl">Real World Routes</Dialog.Title>
              <Dialog.CloseButton>
                <FaSolidXmark />
              </Dialog.CloseButton>
            </div>
            <Dialog.Description>
              <FlightAwareTable {...modalProps} />
            </Dialog.Description>
          </Dialog.Content>
        </div>
      </Dialog.Portal>
    </Dialog.Root>
  );
};
