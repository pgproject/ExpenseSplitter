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
  method: RequestInit["method"],
  url: string = ENDPOINT,
  data?: unknown
): Promise<T> {
  const token = localStorage.getItem("accessToken");

  const response = await fetch(url, {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    ...(data !== undefined && {
      body: JSON.stringify(data),
    }),
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
    return sendRequest("POST", ENDPOINT, data);
  },

  async update(
    data: UpdateExpenseRequest
  ): Promise<UpdateExpenseResponse> {
    return sendRequest("PUT", ENDPOINT, data);
  },

  async remove(
    data: RemoveExpenseRequest
  ): Promise<RemoveExpenseResponse> {
    return sendRequest("DELETE", ENDPOINT, data);
  },

  async getAll(): Promise<GetExpensesResponse[]> {
    return sendRequest("GET");
  },

  async getByGroup(groupId: string): Promise<GetExpensesResponse[]> {
    return sendRequest("GET", `${ENDPOINT}/group/${groupId}`);
  },
};