
export interface CreateGroupSettingsRequest {
  onlyOwnerCanAddMember: boolean;
  onlyOwnerCanRemoveMember: boolean;
}

export interface CreateGroupRequest {
  name: string;
  settings: CreateGroupSettingsRequest;
}

export interface AddMemberRequest {
  newMemberEmail: string;
  existGroupId: string;
}

export interface RemoveMemberRequest {
  emailMemberToRemove: string;
  existGroupId: string;
}

export interface ChangeOwnershipRequest {
  newOwnerOfGroupEmail: string;
  existGroupId: string;
}

export interface CreateGroupResponse {
  groupId: string;
  name: string;
}

export interface AddMemberResponse {
  email: string;
  groupId: string;
}

export interface RemoveMemberResponse {
  email: string;
  groupId: string;
}

export interface ChangeOwnershipResponse {
  newOwnerEmail: string;
  groupId: string;
}

export interface GetGroupsResponse {
  groupId: string;
  name: string;
  currentUserRole: string;
  membersCount: number;
}

export interface GroupMemberResponse {
  userId: string;
  email: string;
  role: string;
}

export interface GroupSettingsResponse {
  onlyOwnerCanAddMember: boolean;
  onlyOwnerCanRemoveMember: boolean;
  defaultCurrency: number;
}

export interface GetGroupDetailsResponse {
  groupId: string;
  name: string;
  ownerId: string;
  members: GroupMemberResponse[];
  settings: GroupSettingsResponse;
}