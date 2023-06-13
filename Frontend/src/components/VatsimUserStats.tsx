import { Component, createResource } from "solid-js";
import wretch from "wretch";

interface VatsimUserStatsProps {
  id: string;
}

export interface UserDetailsRoot {
  userDetails: UserDetails;
  stats: Stats | null;
}

export interface UserDetails {
  id: number;
  rating: Rating;
  pilotRating: Rating;
  militaryRating: Rating;
  suspensionDate: string | null;
  reg_date: string;
  region_id: string;
  division_id: string;
  subdivision_id: string | null;
  lastratingchange: string | null;
}

export interface Rating {
  id: number;
  shortName: string;
  longName: string;
}

export interface Stats {
  id: number;
  atc: number;
  pilot: number;
  s1: number;
  s2: number;
  s3: number;
  c1: number;
  c2: number;
  c3: number;
  i1: number;
  i2: number;
  i3: number;
  sup: number;
  adm: number;
}

const fetchUserStats = async (id: string) => {
  const url = new URL(`v1/vatsim/users/${id}`, import.meta.env.VITE_IDS_API_BASE).toString();
  return (await wretch(url).get().json()) as UserDetailsRoot;
};

export const VatsimUserStats: Component<VatsimUserStatsProps> = (props) => {
  const [stats] = createResource(() => props.id, fetchUserStats);
  return (
    <table class="table-auto text-xs">
      <tbody>
        <tr>
          <td>ATC Rating:</td>
          <td>{stats()?.userDetails.rating.shortName}</td>
        </tr>
        <tr>
          <td>Pilot Rating:</td>
          <td>{stats()?.userDetails.pilotRating.shortName}</td>
        </tr>
        <tr>
          <td>ATC Hours:</td>
          <td>{stats()?.stats?.atc ? Math.round(stats()!.stats!.atc) : 0}</td>
        </tr>
        <tr>
          <td>Pilot Hours:</td>
          <td>{stats()?.stats?.pilot ? Math.round(stats()!.stats!.pilot) : 0}</td>
        </tr>
      </tbody>
    </table>
  );
};
