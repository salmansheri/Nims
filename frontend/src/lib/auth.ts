export const setToken = (token: string) => {
  try {
    const cookieString = `token=${token}; path=/; max-age=604800; SameSite=Strict`;
    document.cookie = cookieString;
    return cookieString;
  } catch (error) {
    return "";
  }
};

export const getToken = (): string | null => {
  if (typeof document === "undefined") return null;

  const cookieString = document.cookie;
  const cookies = cookieString.split(";");

  for (const cookie of cookies) {
    const [name, value] = cookie.trim().split("=");
    if (name === "token") {
      return decodeURIComponent(value);
    }
  }
  return null;
};

export const removeToken = () => {
  document.cookie = "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT;";
};

export const isAuthenticated = (): boolean => {
  return !!getToken();
};
