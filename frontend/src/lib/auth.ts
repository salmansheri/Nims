export const setToken = (token: string) => {
  try {
    localStorage.setItem("token", token);

    const tokenFromLS = localStorage.getItem("token");

    if (tokenFromLS === token) {
      return true;
    }
  } catch (error) {
    return false;
  }
};

export const getToken = (): string | null => {
  const token = localStorage.getItem("token");

  return token;
};

export const removeToken = () => {
  try {
    localStorage.removeItem("token");
    return true;
  } catch (error) {
    return false;
  }
};

export const isAuthenticated = (): boolean => {
  return !!getToken();
};
