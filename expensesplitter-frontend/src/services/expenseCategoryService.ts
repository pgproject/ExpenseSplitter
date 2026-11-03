import type {
  CreateCategoryExpenseRequest,
  CreateExpenseCategoryResponse,
  UpdateExpenseCategoryRequest,
  UpdateExpenseCategoryResponse,
  RemoveCategoryExpenseRequest,
  RemoveExpenseCategoryResponse,
  GetExpenseCategoriesResponse,
} from "../types/expenseCategory";

const API_URL = "http://localhost:32768/api/expense-categories";

async function sendRequest<T>(
  method: string,
  data?: unknown
): Promise<T> {
  const token = localStorage.getItem("accessToken");

  const response = await fetch(API_URL, {
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
    throw new Error("Expense category operation failed.");
  }

  return response.json();
}

export const expenseCategoryService = {
  async getAll(): Promise<GetExpenseCategoriesResponse[]> {
    return sendRequest("GET");
  },

  async create(
    data: CreateCategoryExpenseRequest
  ): Promise<CreateExpenseCategoryResponse> {
    return sendRequest("POST", data);
  },

  async update(
    data: UpdateExpenseCategoryRequest
  ): Promise<UpdateExpenseCategoryResponse> {
    return sendRequest("PUT", data);
  },

  async remove(
    data: RemoveCategoryExpenseRequest
  ): Promise<RemoveExpenseCategoryResponse> {
    return sendRequest("DELETE", data);
  },
};