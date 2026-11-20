
import { useState } from "react";

import { useDashboard } from "../hooks/useDashboard";
import { Plus } from "lucide-react";

import CreateGroupModal from "../components/groups/CreateGroupModal";
import DashboardSummary from "../components/dashboard/DashboardSummary";
import GroupList from "../components/groups/GroupList";
import RecentExpensesList from "../components/expenses/RecentExpensesList";

function DashboardPage() {
  const {
    groups,
    expenses,
    isLoading,
    error,
    refreshGroups,
  } = useDashboard();

  const [isCreateGroupOpen, setIsCreateGroupOpen] = useState(false);

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

        <DashboardSummary
          groupsCount={groups.length}
          expensesCount={expenses.length}
          latestExpenseDate={expenses.length > 0 ? expenses[0].date : null}
          isLoading={isLoading}
        />

        <section className="content-grid">
          <GroupList
            groups={groups}
            isLoading={isLoading}
            onCreateGroup={() => setIsCreateGroupOpen(true)}
          />

          <RecentExpensesList
            expenses={expenses}
            groups={groups}
            isLoading={isLoading}
          />
        </section>
      </main>

      {isCreateGroupOpen && (
        <CreateGroupModal
          onClose={() => setIsCreateGroupOpen(false)}
          onGroupCreated={refreshGroups}
        />
      )}
    </>
  );
}

export default DashboardPage;
