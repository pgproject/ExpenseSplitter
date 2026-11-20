import type { SyntheticEvent } from "react";
import type { GroupMemberResponse } from "../../types/group";

interface GroupMembersPanelProps {
  members: GroupMemberResponse[];
  newMemberEmail: string;
  isAddingMember: boolean;
  addMemberError: string | null;
  addMemberSuccess: string | null;
  onEmailChange: (email: string) => void;
  onAddMember: (event: SyntheticEvent<HTMLFormElement>) => void;
}

function GroupMembersPanel({
  members,
  newMemberEmail,
  isAddingMember,
  addMemberError,
  addMemberSuccess,
  onEmailChange,
  onAddMember,
}: GroupMembersPanelProps) {
  return (
    <div className="panel">
      <div className="panel-header">
        <h3>Members</h3>
        <span className="group-count">{members.length}</span>
      </div>

      {members.map((member) => (
        <div className="member-item" key={member.userId}>
          <div className="avatar">
            {member.email.charAt(0).toUpperCase()}
          </div>

          <div className="item-info">
            <strong>{member.email}</strong>
            <small>
              {member.role === "Owner" ? "Group owner" : "Group member"}
            </small>
          </div>

          <span
            className={
              member.role === "Owner" ? "role-badge owner" : "role-badge"
            }
          >
            {member.role}
          </span>
        </div>
      ))}

      <form onSubmit={onAddMember} className="add-member-form">
        <div className="form-group">
          <label htmlFor="newMemberEmail">Add member by email</label>

          <input
            id="newMemberEmail"
            type="email"
            value={newMemberEmail}
            onChange={(event) => onEmailChange(event.target.value)}
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
  );
}

export default GroupMembersPanel;