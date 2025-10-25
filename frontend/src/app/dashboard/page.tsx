import { DashboardClient } from "@/components/dashboard-client";
import { API_BASE_URL } from "@/lib/api";
import {
  dehydrate,
  HydrationBoundary,
  QueryClient,
} from "@tanstack/react-query";

export default async function Dashboard() {
  const queryClient = new QueryClient();

  await queryClient.prefetchQuery({
    queryKey: ["recent-intrusions"],
    queryFn: async () => {
      const response = await fetch(`${API_BASE_URL}/intrusion/recent`);
      return await response.json();
    },
    retry: false,
  });
  return (
    <HydrationBoundary state={dehydrate(queryClient)}>
      <DashboardClient />
    </HydrationBoundary>
  );
}
