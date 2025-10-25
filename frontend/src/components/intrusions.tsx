"use client";
import { API_BASE_URL } from "@/lib/api";
import { getCookie } from "@/lib/util";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { toast } from "sonner";
import { Layout } from "./layout";
import { motion } from "motion/react";

export const Intrusions = () => {
  const [page, setPage] = useState(1);
  const queryClient = useQueryClient();
  const pageSize = 20;

  const { data: intrusions, isLoading } = useQuery({
    queryKey: ["intrusions", page],
    queryFn: async () => {
      const tokenObj = await getCookie("token");
      const response = await fetch(
        `${API_BASE_URL}/intrusion?page=${page}&pageSize=${pageSize}`,
        {
          headers: {
            Authorization: `Bearer ${tokenObj?.value}`,
          },
        },
      );
      if (!response.ok) {
        throw new Error("Error Fetching Recent intrusions");
      }

      return response.json();
    },
  });

  console.log(intrusions);

  const { mutate: resolveIntrusion, isPending } = useMutation({
    mutationFn: async (id: number) => {
      const tokenObj = await getCookie("token");
      const response = await fetch(`${API_BASE_URL}${id}/resolve`, {
        method: "PATCH",

        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${tokenObj?.value}`,
        },
      });
    },
    onSuccess: () => {
      toast.success("Intrusion resolved successfully");
      queryClient.invalidateQueries({ queryKey: ["intrusions"] });
      queryClient.invalidateQueries({ queryKey: ["dashboard-stats"] });
    },
    onError: (error) => {
      if (error instanceof Error) {
        const err = error as Error;
        toast.success(`Error while resolving Intrusions: ${err.message}`);
      }
    },
  });

  const getSeverityColor = (severity: number) => {
    switch (severity) {
      case 1:
        return "bg-green-100 text-green-800";
      case 2:
        return "bg-blue-100 text-blue-800";
      case 3:
        return "bg-yellow-100 text-yellow-800";
      case 4:
        return "bg-orange-100 text-orange-800";
      case 5:
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const handleResolve = (id: number) => {
    if (confirm("Are you sure you want to mark this intrusion as resolved?")) {
      resolveIntrusion(id);
    }
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <motion.h1
            className="text-3xl font-bold text-gray-300 mb-8"
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            Intrusion Detection Log
          </motion.h1>

          <div className="bg-white shadow-md rounded-lg overflow-hidden">
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Time
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Source → Destination
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Attack Type
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Severity
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {isLoading
                    ? [...Array(10)].map((_, i) => (
                        <tr key={i}>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-4 bg-gray-200 rounded w-24"></div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-4 bg-gray-200 rounded w-32"></div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-4 bg-gray-200 rounded w-20"></div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-6 bg-gray-200 rounded w-16"></div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-6 bg-gray-200 rounded w-20"></div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap animate-pulse">
                            <div className="h-8 bg-gray-200 rounded w-20"></div>
                          </td>
                        </tr>
                      ))
                    : intrusions?.data.map((intrusion: any) => (
                        <motion.tr
                          key={intrusion.id}
                          initial={{ opacity: 0 }}
                          animate={{ opacity: 1 }}
                          className={
                            intrusion.isResolved
                              ? "bg-green-50"
                              : "hover:bg-gray-50"
                          }
                        >
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {new Date(intrusion.detectedAt).toLocaleString()}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm font-medium text-gray-900">
                              {intrusion.sourceIP}:{intrusion.sourcePort}
                            </div>
                            <div className="text-sm text-gray-500">
                              → {intrusion.destinationIP}:
                              {intrusion.destinationPort}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {intrusion.attackType}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span
                              className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getSeverityColor(intrusion.severity)}`}
                            >
                              Level {intrusion.severity}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span
                              className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                                intrusion.isResolved
                                  ? "bg-green-100 text-green-800"
                                  : "bg-yellow-100 text-yellow-800"
                              }`}
                            >
                              {intrusion.isResolved ? "Resolved" : "Active"}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                            {!intrusion.isResolved && (
                              <button
                                onClick={() => handleResolve(intrusion.id)}
                                className="text-green-600 hover:text-green-900 bg-green-100 hover:bg-green-200 px-3 py-1 rounded text-xs font-medium transition-colors"
                              >
                                Resolve
                              </button>
                            )}
                          </td>
                        </motion.tr>
                      ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
              <div className="flex justify-between flex-1 sm:hidden">
                <button
                  onClick={() => setPage(Math.max(1, page - 1))}
                  disabled={page === 1}
                  className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
                >
                  Previous
                </button>
                <button
                  onClick={() => setPage(page + 1)}
                  className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
                >
                  Next
                </button>
              </div>
              <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                <div>
                  <p className="text-sm text-gray-700">
                    Page <span className="font-medium">{page}</span>
                  </p>
                </div>
                <div>
                  <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                    <button
                      onClick={() => setPage(Math.max(1, page - 1))}
                      disabled={page === 1}
                      className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                    >
                      Previous
                    </button>
                    <button
                      onClick={() => setPage(page + 1)}
                      className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50"
                    >
                      Next
                    </button>
                  </nav>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};
