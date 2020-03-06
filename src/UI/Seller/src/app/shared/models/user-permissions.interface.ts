import { ListArgs } from '../services/middleware-api/listArgs.interface';
import { ListUserGroup, ListUserGroupAssignment, UserGroupAssignment } from '@ordercloud/angular-sdk';

export interface IUserPermissionsService {
  getUserGroups(orgID: string, options: ListArgs): Promise<ListUserGroup>;
  listUserAssignments(userID: string, orgID: string): Promise<ListUserGroupAssignment>;
  updateUserUserGroupAssignments(orgID: string, add: UserGroupAssignment[], del: UserGroupAssignment[]): Promise<void>;
  getParentResourceID(): string;
}
