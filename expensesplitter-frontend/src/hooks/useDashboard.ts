
import { useCallback, useEffect, useState } from "react";

import { groupService } from "../services/groupService";
import { expenseService } from "../services/expenseService";

import type { GetGroupsResponse } from "../types/group";
import type { GetExpensesResponse } from "../types/expense";

export function useDashboard() {
  const [groups, setGroups] = useState<GetGroupsResponse[]>([]);
  const [expenses, setExpenses] = useState<GetExpensesResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadDashboard = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const [groupsData, expensesData] = await Promise.all([
        groupService.getAll(),
        expenseService.getAll(),
      ]);

      setGroups(groupsData);

      setExpenses(
        [...expensesData].sort(
          (a, b) =>
            new Date(b.date).getTime() - new Date(a.date).getTime()
        )
      );
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Nie udało się pobrać danych dashboardu."
      );
    } finally {
      setIsLoading(false);
    }
  }, []);

  const refreshGroups = useCallback(async () => {
    try {
      const updatedGroups = await groupService.getAll();
      setGroups(updatedGroups);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Nie udało się odświeżyć grup."
      );
    }
  }, []);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  return {
    groups,
    expenses,
    isLoading,
    error,
    refreshGroups,
  };
}
