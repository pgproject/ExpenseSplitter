import type { SyntheticEvent } from "react";
import { Users } from "lucide-react";

import type { GetGroupDetailsResponse } from "../../types/group";
import type { GetExpenseCategoriesResponse } from "../../types/expenseCategory";
import { Currency } from "../../types/currency";


interface CreateExpenseModalProps {
  group: GetGroupDetailsResponse;
  categories: GetExpenseCategoriesResponse[];

  expenseName: string;
  amount: string;
  categoryId: string;
  currency: Currency;
  date: string;
  participantUserIds: string[];

  isCreatingExpense: boolean;
  createExpenseError: string | null;

  onClose: () => void;
  onSubmit: (event: SyntheticEvent<HTMLFormElement>) => void;
  onExpenseNameChange: (value: string) => void;
  onAmountChange: (value: string) => void;
  onCategoryChange: (value: string) => void;
  onCurrencyChange: (value: Currency) => void;
  onDateChange: (value: string) => void;
  onToggleParticipant: (userId: string) => void;
}

function CreateExpenseModal({
  group,
  categories,
  expenseName,
  amount,
  categoryId,
  currency,
  date,
  participantUserIds,
  isCreatingExpense,
  createExpenseError,
  onClose,
  onSubmit,
  onExpenseNameChange,
  onAmountChange,
  onCategoryChange,
  onCurrencyChange,
  onDateChange,
  onToggleParticipant,
}: CreateExpenseModalProps) {
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal expense-modal"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-header">
          <h3>Add expense</h3>

          <button
            type="button"
            className="modal-close"
            onClick={onClose}
          >
            ×
          </button>
        </div>

        <form onSubmit={onSubmit}>
          <div className="form-group">
            <label htmlFor="expenseName">Expense name</label>

            <input
              id="expenseName"
              type="text"
              value={expenseName}
              onChange={(event) => onExpenseNameChange(event.target.value)}
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
                onChange={(event) => onAmountChange(event.target.value)}
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
                  onCurrencyChange(Number(event.target.value) as Currency)
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
              onChange={(event) => onCategoryChange(event.target.value)}
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
              onChange={(event) => onDateChange(event.target.value)}
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
                    onChange={() => onToggleParticipant(member.userId)}
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
              onClick={onClose}
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
  );
}

export default CreateExpenseModal;