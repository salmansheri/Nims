"use client";
import { usePathname, useRouter } from "next/navigation";
import { motion } from "motion/react";
import Link from "next/link";
import { removeCookie } from "@/lib/util";
import { toast } from "sonner";

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout = ({ children }: LayoutProps) => {
  const pathname = usePathname();
  const router = useRouter();

  const navigation = [
    { name: "Dashboard", href: "/dashboard" },
    { name: "Intrusions", href: "/intrusions" },
    { name: "Alerts", href: "/alerts" },
    { name: "Reports", href: "/reports" },
  ];

  const handleLogout = async () => {
    const isCookieRemoved = await removeCookie("token");
    if (!isCookieRemoved) {
      throw new Error("Something Went while removing token");
    }

    toast.success("Logged out Successfully");

    router.push("/login");
  };

  return (
    <div className="min-h-screen ">
      <nav className="shadow-lg">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex justify-between h-16">
            <div className="flex">
              <div className="flex-shrink-0 flex items-center">
                <h1 className="text-xl font-bold text-gray-200">
                  Network Intrusion Management
                </h1>
              </div>
              <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
                {navigation.map((item) => (
                  <Link
                    key={item.name}
                    href={item.href}
                    className={`${
                      pathname === item.href
                        ? "border-blue-500 text-gray-300"
                        : "border-transparent text-gray-500 hover:border-gray-700 hover:text-gray-200"
                    } inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium`}
                  >
                    {item.name}
                  </Link>
                ))}
              </div>
            </div>
            <div className="flex items-center">
              <button
                type="button"
                onClick={handleLogout}
                className="ml-4 px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main>
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          {children}
        </motion.div>
      </main>
    </div>
  );
};
