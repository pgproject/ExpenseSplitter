
import { useEffect, useState } from "react";
import type { SyntheticEvent, ReactNode } from "react";

import {
  LayoutDashboard,
  Users,
  Receipt,
  Settings,
  Plus,
  Wallet,
} from "lucide-react";

import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  NavLink,
  Link,
} from "react-router-dom";

import "./App.css";

import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import GroupDetailsPage from "./pages/GroupDetailsPage";

import { useAuth } from "./context/AuthContext";
import { groupService } from "./services/groupService";
import { expenseService } from "./services/expenseService";

import type { GetGroupsResponse } from "./types/group";
import type { GetExpensesResponse } from "./types/expense";

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

function ProtectedRoute({ children }: { children: ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <p>Loading...</p>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function DashboardPage() {
  const { user, logout } = useAuth();

  const [groups, setGroups] = useState<GetGroupsResponse[]>([]);
  const [expenses, setExpenses] = useState<GetExpensesResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [isCreateGroupOpen, setIsCreateGroupOpen] = useState(false);
  const [groupName, setGroupName] = useState("");
  const [onlyOwnerCanAddMember, setOnlyOwnerCanAddMember] = useState(false);
  const [onlyOwnerCanRemoveMember, setOnlyOwnerCanRemoveMember] =
    useState(false);
  const [isCreatingGroup, setIsCreatingGroup] = useState(false);
  const [createGroupError, setCreateGroupError] = useState<string | null>(
    null
  );

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

  const handleCreateGroup = async (
    event: SyntheticEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (!groupName.trim()) {
      return;
    }

    try {
      setIsCreatingGroup(true);
      setCreateGroupError(null);

      await groupService.createGroup({
        name: groupName.trim(),
        settings: {
          onlyOwnerCanAddMember,
          onlyOwnerCanRemoveMember,
        },
      });

      const updatedGroups = await groupService.getAll();
      setGroups(updatedGroups);

      setGroupName("");
      setOnlyOwnerCanAddMember(false);
      setOnlyOwnerCanRemoveMember(false);
      setIsCreateGroupOpen(false);
    } catch (error) {
      setCreateGroupError(
        error instanceof Error
          ? error.message
          : "Nie udało się utworzyć grupy."
      );
    } finally {
      setIsCreatingGroup(false);
    }
  };

  const groupNames = new Map(
    groups.map((group) => [group.groupId, group.name])
  );

  const recentExpenses = expenses.slice(0, 5);
  const initials = user?.email?.charAt(0).toUpperCase() ?? "?";

  return (
    <div className="app">
      <aside className="sidebar">
        <h1 className="logo">ExpenseSplitter</h1>

        <nav>
          <NavLink
            to="/"
            className={({ isActive }) => (isActive ? "active" : "")}
          >
            <LayoutDashboard size={20} /> Dashboard
          </NavLink>

          <a>
            <Users size={20} /> My groups
          </a>

          <a>
            <Receipt size={20} /> All expenses
          </a>

          <a>
            <Settings size={20} /> Settings
          </a>
        </nav>

        <div className="profile">
          <div className="avatar">{initials}</div>

          <div>
            <strong>{user?.email ?? "User"}</strong>
            <small>My account</small>

            <button className="text-button" onClick={logout}>
              Log out
            </button>
          </div>
        </div>
      </aside>

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
                onClick={() => {
                  setCreateGroupError(null);
                  setIsCreateGroupOpen(true);
                }}
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
        <div
          className="modal-overlay"
          onClick={() => setIsCreateGroupOpen(false)}
        >
          <div
            className="modal"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="modal-header">
              <h3>Create group</h3>

              <button
                type="button"
                className="modal-close"
                onClick={() => setIsCreateGroupOpen(false)}
              >
                ×
              </button>
            </div>

            <form onSubmit={handleCreateGroup}>
              <div className="form-group">
                <label htmlFor="groupName">Group name</label>

                <input
                  id="groupName"
                  type="text"
                  value={groupName}
                  onChange={(event) => setGroupName(event.target.value)}
                  placeholder="e.g. Apartment, Italy trip..."
                  required
                  maxLength={100}
                />
              </div>

              <div className="form-group">
                <label>Group permissions</label>

                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    checked={onlyOwnerCanAddMember}
                    onChange={(event) =>
                      setOnlyOwnerCanAddMember(event.target.checked)
                    }
                  />
                  Only the owner can add members
                </label>

                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    checked={onlyOwnerCanRemoveMember}
                    onChange={(event) =>
                      setOnlyOwnerCanRemoveMember(event.target.checked)
                    }
                  />
                  Only the owner can remove members
                </label>
              </div>

              {createGroupError && (
                <p className="form-error">{createGroupError}</p>
              )}

              <div className="modal-actions">
                <button
                  type="button"
                  className="secondary-button"
                  onClick={() => setIsCreateGroupOpen(false)}
                >
                  Cancel
                </button>

                <button
                  type="submit"
                  className="primary-button"
                  disabled={isCreatingGroup || !groupName.trim()}
                >
                  {isCreatingGroup ? "Creating..." : "Create group"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <DashboardPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/groups/:groupId"
          element={
            <ProtectedRoute>
              <GroupDetailsPage />
            </ProtectedRoute>
          }
        />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
