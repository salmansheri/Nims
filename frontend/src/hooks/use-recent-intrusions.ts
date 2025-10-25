import { API_BASE_URL } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { getCookie } from "@/lib/util";

export const useRecentIntrusions = () => {
  return useQuery({
    queryKey: ["recent-intrusions"],
    queryFn: async () => {
      const tokenObj = await getCookie("token");
      const response = await fetch(`${API_BASE_URL}/intrusion/recent`, {
        headers: {
          Authorization: `Bearer ${tokenObj?.value}`,
        },
      });
      if (!response.ok) {
        throw new Error("Error Fetching Recent intrusions");
      }

      return response.json();
    },
    retry: false,
  });
};
