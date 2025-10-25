/** biome-ignore-all lint/suspicious/noExplicitAny: some */
"use client";
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { useRouter } from "next/navigation";
import { useRecentIntrusions } from "@/hooks/use-recent-intrusions";
import { Layout } from "./layout";
import { motion } from "motion/react";
import { DashboardStats } from "./dashboard-stats";
import { placeHolderList } from "@/lib/constants";
import { getCookie } from "@/lib/util";
import { SIGNAL_URL } from "@/lib/api";

export const DashboardClient = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [recentAlerts, setRecentAlerts] = useState<any[]>([]);

  const router = useRouter();

  useEffect(() => {
    const gettoken = async () => {
      const token = await getCookie("token");
      console.log("token", token);
    };

    gettoken();
  });

  const { data: recentIntrusions, isLoading } = useRecentIntrusions();

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${SIGNAL_URL}`)
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log("Connected to SignalR");

          connection.on("ReceiveAlert", (alert: any) => {
            setRecentAlerts((prev) => [alert, ...prev.slice(0, 4)]);

            // Show browser notification
            if (Notification.permission === "granted") {
              new Notification("New Security Alert", {
                body: alert.title,
                icon: "/shield.png",
              });
            }
          });
        })
        .catch((err) => console.error("SignalR Connection Error: ", err));
    }

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection]);

  const requestNotificationPermission = () => {
    if ("Notification" in window && Notification.permission === "default") {
      Notification.requestPermission();
    }
  };

  useEffect(() => {
    requestNotificationPermission();
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

  return (
    <Layout>
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <motion.h1
            className="text-3xl font-bold text-gray-300 mb-8"
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            Security Dashboard
          </motion.h1>

          <DashboardStats />

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <motion.div
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              className="shadow-md rounded-lg overflow-hidden"
            >
              <div className="px-6 py-4 border-b border-gray-500">
                <h2 className="text-lg font-semibold text-gray-200">
                  Recent Intrusions
                </h2>
              </div>
              <div className="divide-y divide-gray-500">
                {isLoading
                  ? placeHolderList.map((p) => (
                      <div key={p} className="p-4 animate-pulse">
                        <div className="h-4 bg-gray-600 rounded w-3/4 mb-2"></div>
                        <div className="h-3 bg-gray-600 rounded w-1/2"></div>
                      </div>
                    ))
                  : recentIntrusions?.data.map((intrusion: any) => (
                      <div key={intrusion.id} className="p-4 hover:bg-gray-50">
                        <div className="flex justify-between items-start">
                          <div>
                            <p className="text-sm font-medium text-gray-900">
                              {intrusion.attackType}
                            </p>
                            <p className="text-sm text-gray-500">
                              {intrusion.sourceIP} → {intrusion.destinationIP}
                            </p>
                          </div>
                          <span
                            className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getSeverityColor(intrusion.severity)}`}
                          >
                            Severity {intrusion.severity}
                          </span>
                        </div>
                        <p className="text-xs text-gray-400 mt-1">
                          {new Date(intrusion.detectedAt).toLocaleString()}
                        </p>
                      </div>
                    ))}
              </div>
            </motion.div>

            {/* Recent Alertsi */}
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              className="shadow-md rounded-lg overflow-hidden"
            >
              <div className="px-6 py-4 border-b border-gray-500">
                <h2 className="text-lg font-semibold text-gray-200">
                  Recent Alerts
                </h2>
              </div>
              <div className="divide-y divide-gray-200">
                {recentAlerts.length === 0 ? (
                  <div className="p-4 text-center text-gray-500">
                    No recent alerts
                  </div>
                ) : (
                  recentAlerts.map((alert) => (
                    <div key={alert.id} className="p-4 hover:bg-gray-50">
                      <div className="flex justify-between items-start">
                        <div>
                          <p className="text-sm font-medium text-gray-900">
                            {alert.title}
                          </p>
                          <p className="text-sm text-gray-500">
                            {alert.description}
                          </p>
                        </div>
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getSeverityColor(alert.severity)}`}
                        >
                          Level {alert.severity}
                        </span>
                      </div>
                      <p className="text-xs text-gray-400 mt-1">
                        {new Date(alert.createdAt).toLocaleString()}
                      </p>
                    </div>
                  ))
                )}
              </div>
            </motion.div>
          </div>
        </div>
      </div>
    </Layout>
  );
};
