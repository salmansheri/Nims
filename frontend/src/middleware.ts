import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { getCookie } from "./lib/util";

// This function can be marked `async` if using `await` inside
export async function middleware(request: NextRequest) {
  const tokenObj = await getCookie("token");

  if (!tokenObj || tokenObj.value.length === 0)
    return NextResponse.redirect(new URL("/login", request.url));
}

// See "Matching Paths" below to learn more
export const config = {
  matcher: ["/", "/dashboard/:path*", "/intrusions/:path*"],
};
