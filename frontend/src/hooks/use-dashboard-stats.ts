import { intrusionAPI } from "@/lib/api";

import { useQuery } from "@tanstack/react-query";

export const useDashboardStats = () => {
  return useQuery({
    queryKey: ["dashboard-stats"],
    queryFn: async () => {
      const response = await intrusionAPI.getDashboardStats();

      if (response.status !== 200) {
        throw new Error("Failed to fetch dashboard stats");
      }

      return response.data;
    },
  });
};
