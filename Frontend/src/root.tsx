// @refresh reload
import { Suspense } from "solid-js";
import {
  useLocation,
  A,
  Body,
  ErrorBoundary,
  FileRoutes,
  Head,
  Html,
  Meta,
  Routes,
  Scripts,
  Title,
  Link
} from "solid-start";
import "./root.css";
import { NavBar } from "./components/NavBar";
import { QueryClient, QueryClientProvider } from "@tanstack/solid-query";
import { NavBackProvider } from "./components/NavContext";

const queryClient = new QueryClient();

export default function Root() {
  return (
    <Html lang="en">
      <Head>
        <Title>ZOA IDS</Title>
        <Meta charset="utf-8" />
        <Meta name="viewport" content="width=device-width, initial-scale=1" />
        <Link rel="icon" href="/favicon.ico" />
        <Link rel="preconnect" href="https://rsms.me/" />
        <Link rel="stylesheet" href="https://rsms.me/inter/inter.css" />
        <Link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=IBM%20Plex%20Mono" />
      </Head>
      <Body class="bg-stone-900 text-gray-200">
        <NavBackProvider>
          <NavBar />
          <QueryClientProvider client={queryClient}>
            <Routes>
              <FileRoutes />
            </Routes>
          </QueryClientProvider>
          <Scripts />
        </NavBackProvider>
      </Body>
    </Html>
  );
}
