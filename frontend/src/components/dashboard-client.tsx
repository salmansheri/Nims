"use client";
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { useRouter } from "next/navigation";
import { isAuthenticated } from "@/lib/auth";
import { useRecentIntrusions } from "@/hooks/use-recent-intrusions";

export const DashboardClient = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [recentAlerts, setRecentAlerts] = useState<any[]>([]);

  const router = useRouter();

  // Redirect if not authenticated

  useEffect(() => {
    if (!isAuthenticated) {
      router.push("/login");
    }
  }, [router]);

  const { data: recentIntrusions, isLoading } = useRecentIntrusions();

  useEffect(() => {
    if (!isAuthenticated) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/alertHub")
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

  if (!isAuthenticated) {
    return null;
  }
  return <div></div>;
};
