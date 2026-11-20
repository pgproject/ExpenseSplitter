import { useState } from "react";
import type { SyntheticEvent, Dispatch, SetStateAction } from "react";

import { expenseService } from "../services/expenseService";

import type { GetGroupDetailsResponse } from "../types/group";
import type {
  GetExpensesResponse,
  CreateExpenseRequest,
} from "../types/expense";
import type { GetExpenseCategoriesResponse } from "../types/expenseCategory";

import { Currency } from "../types/currency";

function getTodayDate(): string {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0");
  const day = String(today.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

export function useCreateExpense(
  groupId: string | undefined,
  group: GetGroupDetailsResponse | null,
  categories: GetExpenseCategoriesResponse[],
  setExpenses: Dispatch<SetStateAction<GetExpensesResponse[]>>
) {
  const [isCreateExpenseOpen, setIsCreateExpenseOpen] = useState(false);
  const [expenseName, setExpenseName] = useState("");
  const [amount, setAmount] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [currency, setCurrency] = useState<Currency>(Currency.PLN);
  const [date, setDate] = useState(getTodayDate());
  const [participantUserIds, setParticipantUserIds] = useState<string[]>([]);
  const [isCreatingExpense, setIsCreatingExpense] = useState(false);
  const [createExpenseError, setCreateExpenseError] = useState<string | null>(null);

  const toggleParticipant = (userId: string) => {
    setParticipantUserIds((current) =>
      current.includes(userId)
        ? current.filter((id) => id !== userId)
        : [...current, userId]
    );
  };

  const openCreateExpenseModal = () => {
    if (!group) return;

    setCreateExpenseError(null);
    setExpenseName("");
    setAmount("");
    setCategoryId(categories[0]?.categoryId ?? "");
    setCurrency(group.settings.defaultCurrency as Currency);
    setDate(getTodayDate());
    setParticipantUserIds(group.members.map((member) => member.userId));
    setIsCreateExpenseOpen(true);
  };

  const closeCreateExpenseModal = () => {
    setIsCreateExpenseOpen(false);
    setCreateExpenseError(null);
  };

  const handleCreateExpense = async (
    event: SyntheticEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (!groupId || !group) return;

    const parsedAmount = Number(amount);

    if (!expenseName.trim()) {
      setCreateExpenseError("Podaj nazwę wydatku.");
      return;
    }

    if (!Number.isFinite(parsedAmount) || parsedAmount <= 0) {
      setCreateExpenseError("Podaj prawidłową kwotę.");
      return;
    }

    if (!categoryId) {
      setCreateExpenseError("Wybierz kategorię wydatku.");
      return;
    }

    if (participantUserIds.length === 0) {
      setCreateExpenseError("Wybierz przynajmniej jednego uczestnika.");
      return;
    }

    try {
      setIsCreatingExpense(true);
      setCreateExpenseError(null);

      const request: CreateExpenseRequest = {
        name: expenseName.trim(),
        amount: parsedAmount,
        categoryId,
        currency,
        date,
        allocation: {
          groupId,
          participantUserIds,
        },
      };

      await expenseService.create(request);

      const updatedExpenses = await expenseService.getByGroup(groupId);

      const sortedExpenses = updatedExpenses.sort(
        (a, b) =>
          new Date(b.date).getTime() - new Date(a.date).getTime()
      );

      setExpenses(sortedExpenses);

      setExpenseName("");
      setAmount("");
      setCategoryId(categories[0]?.categoryId ?? "");
      setCurrency(group.settings.defaultCurrency as Currency);
      setDate(getTodayDate());
      setParticipantUserIds(group.members.map((member) => member.userId));

      closeCreateExpenseModal();
    } catch (error) {
      setCreateExpenseError(
        error instanceof Error
          ? error.message
          : "Nie udało się utworzyć wydatku."
      );
    } finally {
      setIsCreatingExpense(false);
    }
  };

  return {
    isCreateExpenseOpen,
    expenseName,
    amount,
    categoryId,
    currency,
    date,
    participantUserIds,
    isCreatingExpense,
    createExpenseError,

    setExpenseName,
    setAmount,
    setCategoryId,
    setCurrency,
    setDate,
    setParticipantUserIds,

    toggleParticipant,
    openCreateExpenseModal,
    closeCreateExpenseModal,
    handleCreateExpense,
  };
}