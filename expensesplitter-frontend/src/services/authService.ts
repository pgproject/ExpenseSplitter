import API_URL from "../config/api";

import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  AuthUser,
} from "../types/auth";

const ENDPOINT = `${API_URL}/api/Auth`;

async function request<T>(
  url: string,
  options: RequestInit,
  errorMessage: string
): Promise<T> {
  const response = await fetch(url, options);

  if (!response.ok) {
    throw new Error(errorMessage);
  }

  return response.json();
}

export const authService = {
  async login(data: LoginRequest): Promise<LoginResponse> {
    return request(
      `${ENDPOINT}/login`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      },
      "Nieprawidłowy email lub hasło."
    );
  },

  async register(data: RegisterRequest): Promise<RegisterResponse> {
    return request(
      `${ENDPOINT}/register`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      },
      "Nie udało się utworzyć konta."
    );
  },

  async getCurrentUser(token: string): Promise<AuthUser> {
    return request(
      `${ENDPOINT}/me`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      },
      "Nie udało się pobrać danych użytkownika."
    );
  },

  async loginWithGoogle(idToken: string): Promise<LoginResponse> {
    return request(
      `${ENDPOINT}/google`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ idToken }),
      },
      "Nie udało się zalogować przez Google."
    );
  },

  async linkGoogleAccount(idToken: string, token: string): Promise<void> {
    const response = await fetch(`${ENDPOINT}/link-google`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ idToken }),
    });

    if (!response.ok) {
      throw new Error("Nie udało się połączyć konta Google.");
    }
  },
};
