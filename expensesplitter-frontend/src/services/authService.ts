import API_URL from "../config/api";

import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  AuthUser,
} from "../types/auth";

const ENDPOINT = `${API_URL}/api/Auth`;

export const authService = {
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await fetch(`${ENDPOINT}/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error("Nieprawidłowy email lub hasło.");
    }

    return response.json();
  },

  async register(data: RegisterRequest): Promise<RegisterResponse> {
    const response = await fetch(`${ENDPOINT}/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error("Nie udało się utworzyć konta.");
    }

    return response.json();
  },

  async getCurrentUser(token: string): Promise<AuthUser> {
    const response = await fetch(`${ENDPOINT}/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error("Nie udało się pobrać danych użytkownika.");
    }

    return response.json();
  },
};