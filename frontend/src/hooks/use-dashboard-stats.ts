import { API_BASE_URL } from "@/lib/api";
import { getCookie } from "@/lib/util";

import { useQuery } from "@tanstack/react-query";

export const useDashboardStats = () => {
  return useQuery({
    queryKey: ["dashboard-stats"],
    queryFn: async () => {
      const token = await getCookie("token");
      const response = await fetch(`${API_BASE_URL}/dashboard/stats`, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status !== 200) {
        throw new Error("Failed to fetch dashboard stats");
      }

      return response.json();
    },
  });
};
