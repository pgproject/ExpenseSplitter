import { useEffect, useState } from "react";

import { Users, Plus, Wallet } from "lucide-react";
import { Link } from "react-router-dom";

import { groupService } from "../services/groupService";
import { expenseService } from "../services/expenseService";

import type { GetGroupsResponse } from "../types/group";
import type { GetExpensesResponse } from "../types/expense";

import CreateGroupModal from "../components/groups/CreateGroupModal";

const currencyCodes: Record<number, string> = {
  0: "PLN",
  1: "EUR",
  2: "USD",
};

function formatAmount(amount: number, currency: number): string {
  const currencyCode = currencyCodes[currency] ?? "PLN";

  return new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: currencyCode,
  }).format(amount);
}

function formatDate(date: string): string {
  return new Date(date).toLocaleDateString("pl-PL");
}

function DashboardPage() {
  const [groups, setGroups] = useState<GetGroupsResponse[]>([]);
  const [expenses, setExpenses] = useState<GetExpensesResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [isCreateGroupOpen, setIsCreateGroupOpen] = useState(false);

  useEffect(() => {
    const loadDashboard = async () => {
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
    };

    loadDashboard();
  }, []);

  const handleGroupCreated = async () => {
    const updatedGroups = await groupService.getAll();
    setGroups(updatedGroups);
  };

  const groupNames = new Map(
    groups.map((group) => [group.groupId, group.name])
  );

  const recentExpenses = expenses.slice(0, 5);

  return (
    <>
      <main className="main">
        <header className="header">
          <div>
            <h2>Dashboard</h2>
            <p>Overview of your shared expenses</p>
          </div>

          <button className="primary-button" disabled>
            <Plus size={18} /> Add expense
          </button>
        </header>

        {error && (
          <div className="panel">
            <p className="negative">{error}</p>
          </div>
        )}

        <section className="summary">
          <div className="summary-card">
            <span>My groups</span>
            <h2>{isLoading ? "..." : groups.length}</h2>
            <small>Groups you belong to</small>
          </div>

          <div className="summary-card">
            <span>All expenses</span>
            <h2>{isLoading ? "..." : expenses.length}</h2>
            <small>Expenses you added or participate in</small>
          </div>

          <div className="summary-card">
            <span>Latest expense</span>
            <h2>
              {isLoading
                ? "..."
                : expenses.length > 0
                  ? formatDate(expenses[0].date)
                  : "—"}
            </h2>
            <small>Most recently added expense</small>
          </div>
        </section>

        <section className="content-grid">
          <div className="panel">
            <div className="panel-header">
              <h3>My groups</h3>

              <button
                className="text-button"
                onClick={() => setIsCreateGroupOpen(true)}
              >
                <Plus size={16} /> Create group
              </button>
            </div>

            {isLoading ? (
              <p>Loading groups...</p>
            ) : groups.length === 0 ? (
              <p>You don't belong to any groups yet.</p>
            ) : (
              groups.map((group) => (
                <Link
                  to={`/groups/${group.groupId}`}
                  className="group-item group-item-link"
                  key={group.groupId}
                >
                  <div className="group-icon">
                    <Users size={20} />
                  </div>

                  <div className="item-info">
                    <strong>{group.name}</strong>
                    <small>
                      {group.membersCount} members · {group.currentUserRole}
                    </small>
                  </div>
                </Link>
              ))
            )}
          </div>

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
                  .map((allocation) =>
                    groupNames.get(allocation.groupId)
                  )
                  .filter(
                    (name): name is string => name !== undefined
                  );

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
        </section>
      </main>

      {isCreateGroupOpen && (
        <CreateGroupModal
          onClose={() => setIsCreateGroupOpen(false)}
          onGroupCreated={handleGroupCreated}
        />
      )}
    </>
  );
}

export default DashboardPage;
