import { formatDate } from "../../utils/formatters";

interface DashboardSummaryProps {
  groupsCount: number;
  expensesCount: number;
  latestExpenseDate: string | null;
  isLoading: boolean;
}

function DashboardSummary({
  groupsCount,
  expensesCount,
  latestExpenseDate,
  isLoading,
}: DashboardSummaryProps) {
  return (
    <section className="summary">
      <div className="summary-card">
        <span>My groups</span>
        <h2>{isLoading ? "..." : groupsCount}</h2>
        <small>Groups you belong to</small>
      </div>

      <div className="summary-card">
        <span>All expenses</span>
        <h2>{isLoading ? "..." : expensesCount}</h2>
        <small>Expenses you added or participate in</small>
      </div>

      <div className="summary-card">
        <span>Latest expense</span>
        <h2>
          {isLoading
            ? "..."
            : latestExpenseDate
              ? formatDate(latestExpenseDate)
              : "—"}
        </h2>
        <small>Most recently added expense</small>
      </div>
    </section>
  );
}

export default DashboardSummary;