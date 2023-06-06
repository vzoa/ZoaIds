import { Component } from "solid-js";
import { A } from "solid-start";

interface SkyVectorLinkProps {
  departure: string;
  arrival: string;
  route: string;
  text: string;
}

export const SkyVectorLink: Component<SkyVectorLinkProps> = (props) => {
  return (
    <a
      href={`https://skyvector.com/?fpl=${props.departure} ${props.route} ${props.arrival}`}
      target="_blank"
      rel="noreferrer noopener"
      class="underline"
    >
      {props.text}
    </a>
  );
};
