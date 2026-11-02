import type { Currency } from "./currency";

export interface CreateExpenseAllocationRequest {
  groupId: string;
  participantUserIds: string[];
}

export interface CreateExpenseRequest {
  name: string;
  amount: number;
  categoryId: string;
  currency: Currency | null;
  date: string | null;
  allocation: CreateExpenseAllocationRequest | null;
}

export interface UpdateExpenseRequest {
  id: string;
  newName: string;
  amount: number;
  categoryId: string;
  currency: Currency;
  newDate: string;
}

export interface RemoveExpenseRequest {
  id: string;
}

export interface CreateExpenseResponse {
  expenseId: string;
}

export interface UpdateExpenseResponse {
  id: string;
}

export interface RemoveExpenseResponse {
  id: string;
}

export interface GetExpensesResponse {
  expenseId: string;
  name: string;
  amount: number;
  categoryId: string;
  userIdAdded: string;
  date: string;
  currency: Currency;
  allocations: ExpenseAllocationResponse[];
}

export interface ExpenseAllocationResponse {
  groupId: string;
  participantUserIds: string[];
}