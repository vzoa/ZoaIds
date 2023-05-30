import { Component, Suspense, createResource } from "solid-js";
import wretch from "wretch";

interface ChartViewerProps {
  id: string;
}

interface ChartsRoot {
  airportName: string;
  faaIdent: string;
  icaoIdent: string;
  charts: Chart[];
}

interface Chart {
  chartSeq: string;
  chartCode: string;
  chartName: string;
  pages: ChartPage[];
}

interface ChartPage {
  pageNumber: number;
  pdfName: string;
  pdfPath: string;
}

const fetchChartsForAirport = async (id: string) => {
  const url = new URL(`v1/charts/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  //const url = `https://api.aviationapi.com/v1/charts?apt=${id}`;
  const json: ChartsRoot = await wretch(url).get().json();
  return json.charts;
};

export const ChartViewer: Component<ChartViewerProps> = (props) => {
  const [charts] = createResource(props.id, fetchChartsForAirport);
  // const groupedCharts = () => {
  //   const acc: Record<string, Chart[]> = {};
  //   if (charts() != undefined) {
  //     for (const chart of charts()!) {
  //       if (!(chart["chart_code"] in acc)) {
  //         acc[chart["chart_code"]] = [chart];
  //       } else {
  //         acc[chart["chart_code"]].push(chart);
  //       }
  //     }
  //   }
  //   return acc;
  // };

  // createEffect(() => {
  //   console.log(JSON.stringify(groupedCharts()));
  // });

  return <Suspense fallback={<p>"Loading"</p>}>{JSON.stringify(charts(), null, 2)}</Suspense>;
};
