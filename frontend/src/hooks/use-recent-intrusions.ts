import { intrusionAPI } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";

export const useRecentIntrusions = () => {
  return useQuery({
    queryKey: ["recent-intrusions"],
    queryFn: async () => {
      const response = await intrusionAPI.getRecentIntrusions();
      console.log("Recent Intrusions: ", response.data);
      return response.data;
    },
  });
};
