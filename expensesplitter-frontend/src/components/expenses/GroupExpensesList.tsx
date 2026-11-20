import { Wallet, Receipt } from "lucide-react";
import type { GetExpensesResponse } from "../../types/expense";
import { formatAmount, formatDate } from "../../utils/formatters";

interface GroupExpensesListProps {
  expenses: GetExpensesResponse[];
}

function GroupExpensesList({ expenses }: GroupExpensesListProps) {
  return (
    <section className="panel group-expenses-panel">
      <div className="panel-header">
        <h3>Group expenses</h3>
        <span className="group-count">{expenses.length}</span>
      </div>

      {expenses.length === 0 ? (
        <div className="empty-group-expenses">
          <Receipt size={36} />
          <strong>No expenses yet</strong>
          <p>There are no expenses assigned to this group.</p>
        </div>
      ) : (
        expenses.map((expense) => (
          <div className="expense-item" key={expense.expenseId}>
            <div className="expense-icon">
              <Wallet size={20} />
            </div>

            <div className="item-info">
              <strong>{expense.name}</strong>
              <small>{formatDate(expense.date)}</small>
            </div>

            <strong>
              {formatAmount(expense.amount, expense.currency)}
            </strong>
          </div>
        ))
      )}
    </section>
  );
}

export default GroupExpensesList;