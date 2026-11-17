
import { useState } from "react";
import type { SyntheticEvent } from "react";

import { groupService } from "../../services/groupService";

interface CreateGroupModalProps {
  onClose: () => void;
  onGroupCreated: () => Promise<void>;
}

function CreateGroupModal({
  onClose,
  onGroupCreated,
}: CreateGroupModalProps) {
  const [groupName, setGroupName] = useState("");
  const [onlyOwnerCanAddMember, setOnlyOwnerCanAddMember] = useState(false);
  const [onlyOwnerCanRemoveMember, setOnlyOwnerCanRemoveMember] =
    useState(false);

  const [isCreating, setIsCreating] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!groupName.trim()) {
      return;
    }

    try {
      setIsCreating(true);
      setError(null);

      await groupService.createGroup({
        name: groupName.trim(),
        settings: {
          onlyOwnerCanAddMember,
          onlyOwnerCanRemoveMember,
        },
      });

      await onGroupCreated();
      onClose();
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Nie udało się utworzyć grupy."
      );
    } finally {
      setIsCreating(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-header">
          <h3>Create group</h3>

          <button
            type="button"
            className="modal-close"
            onClick={onClose}
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit}>
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

          {error && <p className="form-error">{error}</p>}

          <div className="modal-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={onClose}
              disabled={isCreating}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="primary-button"
              disabled={isCreating || !groupName.trim()}
            >
              {isCreating ? "Creating..." : "Create group"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateGroupModal;
