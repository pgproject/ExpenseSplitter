import { useState } from "react";
import type { SyntheticEvent } from "react";

import { groupService } from "../services/groupService";
import type { GetGroupDetailsResponse } from "../types/group";

interface UseAddGroupMemberProps {
  groupId: string | undefined;
  group: GetGroupDetailsResponse | null;
  onGroupUpdated: (group: GetGroupDetailsResponse) => void;
  onMembersUpdated: (userIds: string[]) => void;
}

export function useAddGroupMember({
  groupId,
  group,
  onGroupUpdated,
  onMembersUpdated,
}: UseAddGroupMemberProps) {
  const [newMemberEmail, setNewMemberEmail] = useState("");
  const [isAddingMember, setIsAddingMember] = useState(false);
  const [addMemberError, setAddMemberError] = useState<string | null>(null);
  const [addMemberSuccess, setAddMemberSuccess] = useState<string | null>(null);

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

      onGroupUpdated(updatedGroup);
      onMembersUpdated(
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

  return {
    newMemberEmail,
    isAddingMember,
    addMemberError,
    addMemberSuccess,
    setNewMemberEmail,
    handleAddMember,
  };
}