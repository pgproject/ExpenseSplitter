import { Wallet } from "lucide-react";
import type { GetExpensesResponse } from "../../types/expense";
import type { GetGroupsResponse } from "../../types/group";
import { formatAmount, formatDate } from "../../utils/formatters";

interface RecentExpensesListProps {
  expenses: GetExpensesResponse[];
  groups: GetGroupsResponse[];
  isLoading: boolean;
}

function RecentExpensesList({
  expenses,
  groups,
  isLoading,
}: RecentExpensesListProps) {
  const groupNames = new Map(
    groups.map((group) => [group.groupId, group.name])
  );

  const recentExpenses = expenses.slice(0, 5);

  return (
    <div className="panel">
      <div className="panel-header">
        <h3>Recent expenses</h3>
      </div>

      {isLoading ? (
        <p>Loading expenses...</p>
      ) : recentExpenses.length === 0 ? (
        <p>No expenses yet.</p>
      ) : (
        recentExpenses.map((expense) => {
          const expenseGroups = expense.allocations
            .map((allocation) => groupNames.get(allocation.groupId))
            .filter((name): name is string => name !== undefined);

          const groupLabel =
            expenseGroups.length > 0
              ? expenseGroups.join(", ")
              : "No group assigned";

          return (
            <div className="expense-item" key={expense.expenseId}>
              <div className="expense-icon">
                <Wallet size={20} />
              </div>

              <div className="item-info">
                <strong>{expense.name}</strong>
                <small>
                  {groupLabel} · {formatDate(expense.date)}
                </small>
              </div>

              <strong>
                {formatAmount(expense.amount, expense.currency)}
              </strong>
            </div>
          );
        })
      )}
    </div>
  );
}

export default RecentExpensesList;