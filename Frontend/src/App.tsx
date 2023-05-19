import { createResource, type Component, createSignal } from "solid-js";

import { VatsimPilot } from "./components/interfaces";
import { TimeDisplay, PilotCard, MarkdownRender } from "./components";
import { Checklist } from "./components/Checklist";

const fetchData = async () =>
  (await fetch("https://localhost:7160/api/v1/routes/klax/ksfo")).json();

const App: Component = () => {
  // const [data] = createResource(fetchData);
  const pilot: VatsimPilot = JSON.parse(
    '{"cid":1468087,"name":"Higor Rodrigues SBGL","callsign":"QTR18H","server":"USA-EAST","pilot_rating":0,"military_rating":0,"latitude":37.88317,"longitude":-119.61414,"altitude":38148,"groundspeed":464,"transponder":"2000","heading":180,"qnh_i_hg":30.07,"qnh_mb":1018,"flight_plan":{"flight_rules":"I","aircraft":"B77W/H-VGDW/C","aircraft_faa":"H/B77W/L","aircraft_short":"B77W","departure":"OTHH","arrival":"KLAX","alternate":"KLAS","cruise_tas":"490","altitude":"38000","deptime":"1010","enroute_time":"1625","fuel_time":"1805","remarks":"PBN/A1B1C1D1L1O1S2 DOF/230516 REG/A7BAB EET/OIIX0015 UBBA0147 URRV0219 UUWV0333 ULLL0448 ENOR0615 ENOB0635 BGGL0740 CZEG0952 CZWG1219 CZEG1328 KZLC1357 KZSE1407 KZLC1429 KZOA1508 KZLA1549 SEL/BPAE CODE/06A053 OPR/QTR PER/D RALT/BGTL BGSF TALT/OMDB RMK/TCAS /V/","route":"ALVEN P430 BONAN T430 RAGAS P550 SYZ P574 PEKAM L124 SAV N72 BAKUV N39 LASKA N50 LAKEG N156 GIPEN T747 MOVIT P65 BARUX 7700E 7820N 7840N 7660N DAPAK 7180N 6590N YEK J540 YVC Q959 MEETO Q896 YQL J540 MLP J537 REO J7 REBRG","revision_id":5,"assigned_transponder":"0000"},"logon_time":"2023-05-16T08:47:17.8573754Z","last_updated":"2023-05-17T01:41:12.2211508Z"}'
  );

  const markdown = `# ZOA Info Tool
  ZOA Info Tool is a desktop application to help the controllers of the [Oakland ARTCC on Vatsim](https://oakartcc.org/) quickly access status, routing and airspace information.
  
  <img src="https://user-images.githubusercontent.com/34892440/210297905-652a97d7-ab4f-4788-8f7f-07e419f5ab4c.gif" width=800 />
  
  The app is built using WinUI3 – the latest UI framework from Microsoft – and distributed as an unpackaged, self-contained executable + assemblies.
  
  # Download and Installation
  Download the latest Zip file containing the application: https://github.com/vzoa/info-tool/releases/download/v1.0.0/ZoaInfoTool.v1.0.0.zip
  
  Unzip the folder anywhere you choose and run \`ZoaInfo.exe\` to start the program. The app is self-contained (i.e., includes all dependencies) does not need any installation or separate downloads.
  
  **Requires Windows 10, October 2018 Update or newer**. Requires Windows 11, October 2021 Update for customized title bar.
  
  # Features
  * View real-world D-ATIS for ZOA airports (from https://datis.clowd.io/ API)
  * Search for real-world IFR routes between two airports (from FlightAware)
  * Embedded browser to quickly view routes on SkyVector
  * Search LOAs between ZOA and neighboring ARTCCs for specific routing rules between 2 airports, or for general rules
  * Search the ZOA "alias route" file for prescribed routing between intra-NCT and intra-ZOA airports
  * Get links for all FAA charts for an airport, grouped by chart type
  * Lookup information based on aircraft, airline or airport ICAO code
  * Checks GitHub for new versions and prompts user to download if available
  
  # Functionality Tips & Tricks
  * You can use "numbered" hotkeys (\`Ctrl+Alt+1\`, \`Ctrl+Alt+2\`, \`Ctrl+Alt+3\`, etc.) to quickly change tabs
  * For the routing pages, if you select a row and hit \`c\` on your keyboard, the route will be copied to your clipboard
  * You can use the \`Enter\` key on your keyboard to "submit" the search / lookup forms on most pages instead of hitting the \`Go\` button
  * ZOA Info Tool saves window size and position when you resize or move the window, and restores it when you start the program
  
  # Todos
  * Quickly show general LOA information
  * Add quick links to SOPs
  * View VATSIM D-ATIS for ZOA and neighboring ARTCCs
  
  # Bugs?
  Submit issues on GitHub (https://github.com/vzoa/info-tool/issues) or on the ZOA Discord.
  `;

  const [item, setItem] = createSignal("test");
  //setInterval(() => setItem((prev) => prev + "1"), 1000);

  const checklist = () => [item(), "test2", "test3"];

  return (
    // <span>
    //   {JSON.stringify(data(), null, 2)}
    // </span>
    <>
      {/* <PilotCard pilot={pilot} />
      <TimeDisplay />
      <MarkdownRender markdown={markdown} /> */}
      {/* <Checklist items={[item(), "test2", "test3"]} /> */}
      <div>{item()}</div>
      <Checklist items={[item(), "Weather", "test3"]} />
    </>
  );
};

export default App;
