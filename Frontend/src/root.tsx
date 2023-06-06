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
  Title
} from "solid-start";
import "./root.css";
import { NavBar } from "./components/NavBar";
import { QueryClient, QueryClientProvider } from "@tanstack/solid-query";

const queryClient = new QueryClient();

export default function Root() {
  return (
    <Html lang="en">
      <Head>
        <Title>SolidStart - With TailwindCSS</Title>
        <Meta charset="utf-8" />
        <Meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="preconnect" href="https://rsms.me/" />
        <link rel="stylesheet" href="https://rsms.me/inter/inter.css" />
        <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=IBM%20Plex%20Mono" />
      </Head>
      <Body class="bg-stone-900 text-gray-200">
        <NavBar />
        <QueryClientProvider client={queryClient}>
          <Routes>
            <FileRoutes />
          </Routes>
        </QueryClientProvider>
        <Scripts />
      </Body>
    </Html>
  );
}
