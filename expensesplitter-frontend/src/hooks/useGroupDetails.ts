import { useEffect, useState } from "react";

import { groupService } from "../services/groupService";
import { expenseService } from "../services/expenseService";
import { expenseCategoryService } from "../services/expenseCategoryService";

import type { GetGroupDetailsResponse } from "../types/group";
import type { GetExpensesResponse } from "../types/expense";
import type { GetExpenseCategoriesResponse } from "../types/expenseCategory";

export function useGroupDetails(groupId: string | undefined) {
  const [group, setGroup] = useState<GetGroupDetailsResponse | null>(null);
  const [expenses, setExpenses] = useState<GetExpensesResponse[]>([]);
  const [categories, setCategories] = useState<GetExpenseCategoriesResponse[]>([]);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!groupId) return;

    const loadGroup = async () => {
      try {
        setIsLoading(true);
        setError(null);

        const [groupData, expensesData, categoriesData] = await Promise.all([
          groupService.getById(groupId),
          expenseService.getByGroup(groupId),
          expenseCategoryService.getAll(),
        ]);

        setGroup(groupData);
        setCategories(categoriesData);

        const sortedExpenses = expensesData.sort(
          (a, b) =>
            new Date(b.date).getTime() - new Date(a.date).getTime()
        );

        setExpenses(sortedExpenses);
      } catch (error) {
        setError(
          error instanceof Error
            ? error.message
            : "Nie udało się pobrać danych grupy."
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadGroup();
  }, [groupId]);

  return {
    group,
    setGroup,
    expenses,
    setExpenses,
    categories,
    isLoading,
    error,
  };
}