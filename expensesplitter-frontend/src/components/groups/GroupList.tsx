import { Users, Plus } from "lucide-react";
import { Link } from "react-router-dom";
import type { GetGroupsResponse  } from "../../types/group";

interface GroupListProps {
  groups: GetGroupsResponse[];
  isLoading: boolean;
  onCreateGroup: () => void;
}

function GroupList({
  groups,
  isLoading,
  onCreateGroup,
}: GroupListProps) {
  return (
    <div className="panel">
      <div className="panel-header">
        <h3>My groups</h3>

        <button
          className="text-button"
          onClick={onCreateGroup}
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
  );
}

export default GroupList;