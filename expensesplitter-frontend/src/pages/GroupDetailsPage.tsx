import { useParams } from "react-router-dom";

import GroupSettingsPanel from "../components/groups/GroupSettingsPanel";
import GroupMembersPanel from "../components/groups/GroupMembersPanel";
import GroupExpensesList from "../components/expenses/GroupExpensesList";
import CreateExpenseModal from "../components/expenses/CreateExpenseModal";
import GroupDetailsHeader from "../components/groups/GroupDetailsHeader";

import { getCurrencyCode } from "../utils/formatters";
import { useGroupDetails } from "../hooks/useGroupDetails";
import { useAddGroupMember } from "../hooks/useAddGroupMember";
import { useCreateExpense } from "../hooks/useCreateExpense";

function GroupDetailsPage() {
  const { groupId } = useParams<{ groupId: string }>();

  const {
    group,
    setGroup,
    expenses,
    setExpenses,
    categories,
    isLoading,
    error,
  } = useGroupDetails(groupId);

  const {
    isCreateExpenseOpen,
    expenseName,
    amount,
    categoryId,
    currency,
    date,
    participantUserIds,
    isCreatingExpense,
    createExpenseError,
    setExpenseName,
    setAmount,
    setCategoryId,
    setCurrency,
    setDate,
    setParticipantUserIds,
    toggleParticipant,
    openCreateExpenseModal,
    closeCreateExpenseModal,
    handleCreateExpense,
  } = useCreateExpense(groupId, group, categories, setExpenses);


  const {
    newMemberEmail,
    isAddingMember,
    addMemberError,
    addMemberSuccess,
    setNewMemberEmail,
    handleAddMember,
  } = useAddGroupMember({
    groupId,
    group,
    onGroupUpdated: setGroup,
    onMembersUpdated: setParticipantUserIds,
  });
  
  return (
    <>
      <main className="main">
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
            <GroupDetailsHeader
              groupName={group.name}
              membersCount={group.members.length}
              onAddExpense={openCreateExpenseModal}
            />

            <section className="group-details-grid">
              <GroupMembersPanel
                members={group.members}
                newMemberEmail={newMemberEmail}
                isAddingMember={isAddingMember}
                addMemberError={addMemberError}
                addMemberSuccess={addMemberSuccess}
                onEmailChange={setNewMemberEmail}
                onAddMember={handleAddMember}
              />

              <GroupSettingsPanel
                settings={group.settings}
                currencyLabel={
                  getCurrencyCode(group.settings.defaultCurrency)
                }
              />
            </section>

            <GroupExpensesList expenses={expenses} />
          </>
        ) : null}
      </main>

        {isCreateExpenseOpen && group && (
          <CreateExpenseModal
            group={group}
            categories={categories}
            expenseName={expenseName}
            amount={amount}
            categoryId={categoryId}
            currency={currency}
            date={date}
            participantUserIds={participantUserIds}
            isCreatingExpense={isCreatingExpense}
            createExpenseError={createExpenseError}
            onClose={closeCreateExpenseModal}
            onSubmit={handleCreateExpense}
            onExpenseNameChange={setExpenseName}
            onAmountChange={setAmount}
            onCategoryChange={setCategoryId}
            onCurrencyChange={setCurrency}
            onDateChange={setDate}
            onToggleParticipant={toggleParticipant}
          />
        )}
      </>
    );
  }

export default GroupDetailsPage;