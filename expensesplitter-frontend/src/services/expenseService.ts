import API_URL from "../config/api";

import type {
  CreateExpenseRequest,
  CreateExpenseResponse,
  UpdateExpenseRequest,
  UpdateExpenseResponse,
  RemoveExpenseRequest,
  RemoveExpenseResponse,
  GetExpensesResponse,
} from "../types/expense";

const ENDPOINT = `${API_URL}/api/expenses`;

async function sendRequest<T>(
  method: string,
  data: unknown
): Promise<T> {
  const token = localStorage.getItem("accessToken");

  const response = await fetch(ENDPOINT, {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    throw new Error("Expense operation failed.");
  }

  return response.json();
}

export const expenseService = {
  async create(
    data: CreateExpenseRequest
  ): Promise<CreateExpenseResponse> {
    return sendRequest("POST", data);
  },

  async update(
    data: UpdateExpenseRequest
  ): Promise<UpdateExpenseResponse> {
    return sendRequest("PUT", data);
  },

  async remove(
    data: RemoveExpenseRequest
  ): Promise<RemoveExpenseResponse> {
    return sendRequest("DELETE", data);
  },

  async getAll(): Promise<GetExpensesResponse[]> {
    const token = localStorage.getItem("accessToken");

    const response = await fetch(ENDPOINT, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error("Nie udało się pobrać wydatków.");
    }

    return response.json();
  },

  async getByGroup(groupId: string): Promise<GetExpensesResponse[]> {
    const token = localStorage.getItem("accessToken");

    const response = await fetch(`${ENDPOINT}/group/${groupId}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error("Nie udało się pobrać wydatków grupy.");
    }

    return response.json();
  },
};
