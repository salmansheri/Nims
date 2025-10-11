"use client";

import { isAuthenticated } from "@/lib/auth";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export const HomeClient = () => {
  const router = useRouter();

  useEffect(() => {
    if (isAuthenticated()) {
      router.push("/dashboard");
    } else {
      router.push("/login");
    }
  }, [router]);

  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
    </div>
  );
};
