import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import { authService } from "../services/authService";
import type { AuthUser, LoginRequest } from "../types/auth";

interface AuthContextType {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (data: LoginRequest) => Promise<void>;
  loginWithGoogle: (idToken: string) => Promise<void>;
  logout: () => void;
  linkGoogleAccount: (idToken: string) => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const TOKEN_KEY = "accessToken";

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const initializeAuth = async () => {
      const token = localStorage.getItem(TOKEN_KEY);

      if (!token) {
        setIsLoading(false);
        return;
      }

      try {
        const currentUser = await authService.getCurrentUser(token);
        setUser(currentUser);
      } catch {
        localStorage.removeItem(TOKEN_KEY);
        setUser(null);
      } finally {
        setIsLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = async (data: LoginRequest): Promise<void> => {
    const response = await authService.login(data);

    localStorage.setItem(TOKEN_KEY, response.accessToken);

    const currentUser = await authService.getCurrentUser(response.accessToken);
    setUser(currentUser);
  };

  const logout = (): void => {
    localStorage.removeItem(TOKEN_KEY);
    setUser(null);
  };

  const loginWithGoogle = async (idToken: string): Promise<void> => {
    const response = await authService.loginWithGoogle(idToken);

    localStorage.setItem(TOKEN_KEY, response.accessToken);

    const currentUser = await authService.getCurrentUser(response.accessToken);
    setUser(currentUser);
  };

  const linkGoogleAccount = async (idToken: string): Promise<void> => {
    const token = localStorage.getItem(TOKEN_KEY);

    if (!token) {
      throw new Error("User is not authenticated.");
    }

    await authService.linkGoogleAccount(idToken, token);

    const updatedUser = await authService.getCurrentUser(token);
    setUser(updatedUser);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: user !== null,
        isLoading,
        login,
        loginWithGoogle,
        logout,
        linkGoogleAccount,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
}