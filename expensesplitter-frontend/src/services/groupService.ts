
import type {
  CreateGroupRequest,
  CreateGroupResponse,
  AddMemberRequest,
  AddMemberResponse,
  RemoveMemberRequest,
  RemoveMemberResponse,
  ChangeOwnershipRequest,
  ChangeOwnershipResponse,
  GetGroupsResponse,
  GetGroupDetailsResponse,
} from "../types/group";

const API_URL = "http://localhost:32768/api/groups";

async function sendRequest<T>(
  endpoint: string,
  method: string,
  data?: unknown
): Promise<T> {
  const token = localStorage.getItem("accessToken");

  const response = await fetch(`${API_URL}${endpoint}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    ...(data !== undefined && {
      body: JSON.stringify(data),
    }),
  });

  if (!response.ok) {
    throw new Error("Group operation failed.");
  }

  return response.json();
}

export const groupService = {
  async getAll(): Promise<GetGroupsResponse[]> {
    return sendRequest("", "GET");
  },

  async createGroup(
    data: CreateGroupRequest
  ): Promise<CreateGroupResponse> {
    return sendRequest("", "POST", data);
  },

  async addMember(
    data: AddMemberRequest
  ): Promise<AddMemberResponse> {
    return sendRequest("/members", "POST", data);
  },

  async removeMember(
    data: RemoveMemberRequest
  ): Promise<RemoveMemberResponse> {
    return sendRequest("/members", "DELETE", data);
  },

  async changeOwnership(
    data: ChangeOwnershipRequest
  ): Promise<ChangeOwnershipResponse> {
    return sendRequest("/ownership", "PUT", data);
  },

  async getById(groupId: string): Promise<GetGroupDetailsResponse> {
    return sendRequest(`/${groupId}`, "GET");
  },
};
