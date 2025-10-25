"use client";
import type React from "react";
import { motion } from "motion/react";
import { useDashboardStats } from "@/hooks/use-dashboard-stats";

interface StatsCardProps {
  title: string;
  color: string;
  icon: string;
  value: any;
}

export const StatsCard: React.FC<StatsCardProps> = ({ title, color, icon }) => {
  return (
    <motion.div
      whileHover={{ scale: 1.05 }}
      className={`bg-neutral-800 rounded-lg shadow-md p-6 ${color}`}
    >
      <div className="flex items-center">
        <div className="flex-shrink-0">
          <span className="text-2xl">{icon}</span>
        </div>
        <div className="ml-4">
          <h3 className="text-lg font-medium text-gray-300">{title}</h3>
          {/* <p className="text-3xl font-bold text-gray-200">{value}</p> */}
        </div>
      </div>
    </motion.div>
  );
};

export const DashboardStats = () => {
  const { data: stats, isLoading } = useDashboardStats();
  const placeholderList = [1, 2, 3, 4];

  console.log("state = ", stats);

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {placeholderList.map((p) => (
          <div
            key={p}
            className="bg-white rounded-lg shadow-md p-6 animate-pulse"
          >
            <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
            <div className="h-8 bg-gray-200 rounded w-1/2"></div>
          </div>
        ))}
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <StatsCard
        value={stats?.data.totalIntrusions || 0}
        title="Total Intrusions"
        color="border-l-4 border-blue-500"
        icon="🛡"
      />
      <StatsCard
        value={stats?.data.resolvedIntrusions || 0}
        title="Resolved"
        color="border-l-4 border-green-500"
        icon="✅"
      />
      <StatsCard
        value={stats?.data.highSeverityIntrusions || 0}
        title="High Severity"
        color="border-l-4 border-red-500"
        icon="🚨"
      />
      <StatsCard
        value={stats?.data.todayIntrusions || 0}
        title="Today"
        color="border-l-4 border-yellow-500"
        icon="📊"
      />
    </div>
  );
};
