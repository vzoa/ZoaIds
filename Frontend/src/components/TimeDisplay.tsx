import { Component, createMemo, mergeProps } from "solid-js";
import { createDateNow } from "@solid-primitives/date";

interface TimeDisplayProps {
  updateInterval?: number;
}

export const TimeDisplay: Component<TimeDisplayProps> = (props) => {
  const defaultProps: TimeDisplayProps = {
    updateInterval: 200
  };
  const merged = mergeProps(defaultProps, props);
  const [now] = createDateNow(merged.updateInterval);
  const zuluStr = createMemo(() => now().toISOString());

  return (
    <>
      <span class="font-mono">{zuluStr().slice(11, 13)}</span>
      <span class="font-mono">:</span>
      <span class="font-mono">{zuluStr().slice(14, 16)}</span>
      <span class="font-mono">:</span>
      <span class="font-mono">{zuluStr().slice(17, 19)}</span>
    </>
  );
};
