
import { useEffect, useState } from "react";
import type { SyntheticEvent } from "react";
import { Link, useParams } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  Receipt,
  Settings,
  Wallet,
  ArrowLeft,
  Plus,
} from "lucide-react";

import { useAuth } from "../context/AuthContext";
import { groupService } from "../services/groupService";
import { expenseService } from "../services/expenseService";
import { expenseCategoryService } from "../services/expenseCategoryService";

import type { GetGroupDetailsResponse } from "../types/group";
import type { GetExpensesResponse, CreateExpenseRequest } from "../types/expense";
import type { GetExpenseCategoriesResponse } from "../types/expenseCategory";
import { Currency } from "../types/currency";

const currencyCodes: Record<number, string> = {
  0: "PLN",
  1: "EUR",
  2: "USD",
};

function formatAmount(amount: number, currency: number): string {
  return new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: currencyCodes[currency] ?? "PLN",
  }).format(amount);
}

function formatDate(date: string): string {
  return new Date(date).toLocaleDateString("pl-PL");
}

function getTodayDate(): string {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0");
  const day = String(today.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

function GroupDetailsPage() {
  const { groupId } = useParams<{ groupId: string }>();
  const { user, logout } = useAuth();

  const [group, setGroup] = useState<GetGroupDetailsResponse | null>(null);
  const [expenses, setExpenses] = useState<GetExpensesResponse[]>([]);
  const [categories, setCategories] = useState<GetExpenseCategoriesResponse[]>([]);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [isCreateExpenseOpen, setIsCreateExpenseOpen] = useState(false);
  const [expenseName, setExpenseName] = useState("");
  const [amount, setAmount] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [currency, setCurrency] = useState<Currency>(Currency.PLN);
  const [date, setDate] = useState(getTodayDate());
  const [participantUserIds, setParticipantUserIds] = useState<string[]>([]);
  const [isCreatingExpense, setIsCreatingExpense] = useState(false);
  const [createExpenseError, setCreateExpenseError] = useState<string | null>(null);

  const [newMemberEmail, setNewMemberEmail] = useState("");
  const [isAddingMember, setIsAddingMember] = useState(false);
  const [addMemberError, setAddMemberError] = useState<string | null>(null);
  const [addMemberSuccess, setAddMemberSuccess] = useState<string | null>(null);

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

        setParticipantUserIds(
          groupData.members.map((member) => member.userId)
        );

        setCategoryId(categoriesData[0]?.categoryId ?? "");

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

  const toggleParticipant = (userId: string) => {
    setParticipantUserIds((current) =>
      current.includes(userId)
        ? current.filter((id) => id !== userId)
        : [...current, userId]
    );
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

  const handleAddMember = async (
    event: SyntheticEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (!groupId || !group || !newMemberEmail.trim()) return;

    try {
      setIsAddingMember(true);
      setAddMemberError(null);
      setAddMemberSuccess(null);

      const response = await groupService.addMember({
        newMemberEmail: newMemberEmail.trim(),
        existGroupId: groupId,
      });

      const updatedGroup = await groupService.getById(groupId);

      setGroup(updatedGroup);
      setParticipantUserIds(
        updatedGroup.members.map((member) => member.userId)
      );

      setNewMemberEmail("");
      setAddMemberSuccess(`Added ${response.email} to the group.`);
    } catch (error) {
      setAddMemberError(
        error instanceof Error
          ? error.message
          : "Failed to add group member."
      );
    } finally {
      setIsAddingMember(false);
    }
  };

  const initials = user?.email?.charAt(0).toUpperCase() ?? "?";

  return (
    <div className="app">
      <aside className="sidebar">
        <h1 className="logo">ExpenseSplitter</h1>

        <nav>
          <Link to="/">
            <LayoutDashboard size={20} /> Dashboard
          </Link>

          <Link to="/" className="active">
            <Users size={20} /> My groups
          </Link>

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
        <div className="group-back">
          <Link to="/" className="text-button">
            <ArrowLeft size={17} /> Back to dashboard
          </Link>
        </div>

        {isLoading ? (
          <div className="panel">
            <p>Loading group...</p>
          </div>
        ) : error ? (
          <div className="panel">
            <p className="negative">{error}</p>
          </div>
        ) : group ? (
          <>
            <header className="header group-page-header">
              <div>
                <h2>{group.name}</h2>
                <p>
                  {group.members.length} members · Group details
                </p>
              </div>

              <button
                className="primary-button"
                onClick={() => {
                  setCreateExpenseError(null);
                  setExpenseName("");
                  setAmount("");
                  setCategoryId(categories[0]?.categoryId ?? "");
                  setCurrency(group.settings.defaultCurrency as Currency);
                  setDate(getTodayDate());
                  setParticipantUserIds(
                    group.members.map((member) => member.userId)
                  );
                  setIsCreateExpenseOpen(true);
                }}
              >
                <Plus size={18} /> Add expense
              </button>
            </header>

            <section className="group-details-grid">
              <div className="panel">
                <div className="panel-header">
                  <h3>Members</h3>
                  <span className="group-count">
                    {group.members.length}
                  </span>
                </div>

                {group.members.map((member) => (
                  <div className="member-item" key={member.userId}>
                    <div className="avatar">
                      {member.email.charAt(0).toUpperCase()}
                    </div>

                    <div className="item-info">
                      <strong>{member.email}</strong>
                      <small>
                        {member.role === "Owner"
                          ? "Group owner"
                          : "Group member"}
                      </small>
                    </div>

                    <span
                      className={
                        member.role === "Owner"
                          ? "role-badge owner"
                          : "role-badge"
                      }
                    >
                      {member.role}
                    </span>
                  </div>
                ))}

                <form onSubmit={handleAddMember} className="add-member-form">
                  <div className="form-group">
                    <label htmlFor="newMemberEmail">Add member by email</label>

                    <input
                      id="newMemberEmail"
                      type="email"
                      value={newMemberEmail}
                      onChange={(event) => setNewMemberEmail(event.target.value)}
                      placeholder="user@example.com"
                      required
                    />
                  </div>

                  {addMemberError && (
                    <p className="form-error">{addMemberError}</p>
                  )}

                  {addMemberSuccess && (
                    <p className="form-success">{addMemberSuccess}</p>
                  )}

                  <button
                    type="submit"
                    className="primary-button"
                    disabled={isAddingMember || !newMemberEmail.trim()}
                  >
                    {isAddingMember ? "Adding..." : "Add member"}
                  </button>
                </form>
              </div>

              <div className="panel">
                <div className="panel-header">
                  <h3>Group settings</h3>
                </div>

                <div className="setting-item">
                  <span>Only owner can add members</span>
                  <strong>
                    {group.settings.onlyOwnerCanAddMember ? "Yes" : "No"}
                  </strong>
                </div>

                <div className="setting-item">
                  <span>Only owner can remove members</span>
                  <strong>
                    {group.settings.onlyOwnerCanRemoveMember ? "Yes" : "No"}
                  </strong>
                </div>

                <div className="setting-item">
                  <span>Default currency</span>
                  <strong>
                    {currencyCodes[group.settings.defaultCurrency] ?? "PLN"}
                  </strong>
                </div>
              </div>
            </section>

            <section className="panel group-expenses-panel">
              <div className="panel-header">
                <h3>Group expenses</h3>
                <span className="group-count">{expenses.length}</span>
              </div>

              {expenses.length === 0 ? (
                <div className="empty-group-expenses">
                  <Receipt size={36} />
                  <strong>No expenses yet</strong>
                  <p>
                    There are no expenses assigned to this group.
                  </p>
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
          </>
        ) : null}
      </main>

      {isCreateExpenseOpen && group && (
        <div
          className="modal-overlay"
          onClick={closeCreateExpenseModal}
        >
          <div
            className="modal expense-modal"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="modal-header">
              <h3>Add expense</h3>

              <button
                type="button"
                className="modal-close"
                onClick={closeCreateExpenseModal}
              >
                ×
              </button>
            </div>

            <form onSubmit={handleCreateExpense}>
              <div className="form-group">
                <label htmlFor="expenseName">Expense name</label>

                <input
                  id="expenseName"
                  type="text"
                  value={expenseName}
                  onChange={(event) => setExpenseName(event.target.value)}
                  placeholder="e.g. Groceries, Hotel..."
                  required
                  maxLength={100}
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="expenseAmount">Amount</label>

                  <input
                    id="expenseAmount"
                    type="number"
                    min="0.01"
                    step="0.01"
                    value={amount}
                    onChange={(event) => setAmount(event.target.value)}
                    placeholder="0.00"
                    required
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="expenseCurrency">Currency</label>

                  <select
                    id="expenseCurrency"
                    value={currency}
                    onChange={(event) =>
                      setCurrency(Number(event.target.value) as Currency)
                    }
                  >
                    <option value={Currency.PLN}>PLN</option>
                    <option value={Currency.EUR}>EUR</option>
                    <option value={Currency.USD}>USD</option>
                  </select>
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="expenseCategory">Category</label>

                <select
                  id="expenseCategory"
                  value={categoryId}
                  onChange={(event) => setCategoryId(event.target.value)}
                  required
                >
                  <option value="">Select category</option>

                  {categories.map((category) => (
                    <option
                      key={category.categoryId}
                      value={category.categoryId}
                    >
                      {category.categoryName}
                    </option>
                  ))}
                </select>

                {categories.length === 0 && (
                  <p className="form-hint">
                    You don't have any available expense categories.
                  </p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="expenseDate">Date</label>

                <input
                  id="expenseDate"
                  type="date"
                  value={date}
                  onChange={(event) => setDate(event.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label>Participants</label>

                <div className="participants-list">
                  {group.members.map((member) => (
                    <label
                      className="checkbox-label"
                      key={member.userId}
                    >
                      <input
                        type="checkbox"
                        checked={participantUserIds.includes(member.userId)}
                        onChange={() => toggleParticipant(member.userId)}
                      />
                      {member.email}
                      {member.role === "Owner" ? " (Owner)" : ""}
                    </label>
                  ))}
                </div>
              </div>

              <div className="expense-group-info">
                <Users size={17} />
                Expense will be assigned to group: <strong>{group.name}</strong>
              </div>

              {createExpenseError && (
                <p className="form-error">{createExpenseError}</p>
              )}

              <div className="modal-actions">
                <button
                  type="button"
                  className="secondary-button"
                  onClick={closeCreateExpenseModal}
                  disabled={isCreatingExpense}
                >
                  Cancel
                </button>

                <button
                  type="submit"
                  className="primary-button"
                  disabled={
                    isCreatingExpense ||
                    !expenseName.trim() ||
                    !amount ||
                    !categoryId ||
                    participantUserIds.length === 0
                  }
                >
                  {isCreatingExpense ? "Saving..." : "Save expense"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default GroupDetailsPage;
