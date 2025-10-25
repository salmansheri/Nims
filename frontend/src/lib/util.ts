"use server";
import { cookies } from "next/headers";

export async function setCookies(token: string): Promise<boolean> {
  const cookieStore = await cookies();

  cookieStore.set({
    name: "token",
    value: token,
    httpOnly: true,
  });

  const hasCookie = cookieStore.has("token");

  if (!hasCookie) return false;

  return true;
}

export async function getCookie(key: string) {
  const cookieStore = await cookies();
  const token = cookieStore.get(key);

  return token;
}

export async function removeCookie(key: string) {
  (await cookies()).delete(key);
}
