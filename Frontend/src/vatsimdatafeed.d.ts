export interface VatsimDatafeedRoot {
  general: General;
  pilots: Pilot[];
  controllers: Controller[];
  atis: Atis[];
  servers: Server[];
  prefiles: Prefile[];
  facilities: Facility[];
  ratings: Rating[];
  pilot_ratings: PilotRating[];
  military_ratings: MilitaryRating[];
}

export interface General {
  version: number;
  reload: number;
  update: string;
  update_timestamp: string;
  connected_clients: number;
  unique_users: number;
}

export interface Pilot {
  cid: number;
  name: string;
  callsign: string;
  server: string;
  pilot_rating: number;
  military_rating: number;
  latitude: number;
  longitude: number;
  altitude: number;
  groundspeed: number;
  transponder: string;
  heading: number;
  qnh_i_hg: number;
  qnh_mb: number;
  flight_plan?: FlightPlan;
  logon_time: string;
  last_updated: string;
}

export interface FlightPlan {
  flight_rules: string;
  aircraft: string;
  aircraft_faa: string;
  aircraft_short: string;
  departure: string;
  arrival: string;
  alternate: string;
  cruise_tas: string;
  altitude: string;
  deptime: string;
  enroute_time: string;
  fuel_time: string;
  remarks: string;
  route: string;
  revision_id: number;
  assigned_transponder: string;
}

export interface Controller {
  cid: number;
  name: string;
  callsign: string;
  frequency: string;
  facility: number;
  rating: number;
  server: string;
  visual_range: number;
  text_atis?: string[];
  last_updated: string;
  logon_time: string;
}

export interface Atis extends Controller {
  cid: number;
  name: string;
  callsign: string;
  frequency: string;
  facility: number;
  rating: number;
  server: string;
  visual_range: number;
  atis_code?: string;
  text_atis?: string[];
  last_updated: string;
  logon_time: string;
}

export interface Server {
  ident: string;
  hostname_or_ip: string;
  location: string;
  name: string;
  clients_connection_allowed: number;
  client_connections_allowed: boolean;
  is_sweatbox: boolean;
}

export interface Prefile {
  cid: number;
  name: string;
  callsign: string;
  flight_plan: FlightPlan;
  last_updated: string;
}

export interface Facility {
  id: number;
  short: string;
  long: string;
}

export interface Rating {
  id: number;
  short: string;
  long: string;
}

export interface PilotRating {
  id: number;
  short_name: string;
  long_name: string;
}

export interface MilitaryRating {
  id: number;
  short_name: string;
  long_name: string;
}
