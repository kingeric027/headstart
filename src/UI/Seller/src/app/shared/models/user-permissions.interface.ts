import { UserGroup, UserGroupAssignment, ListPage } from '@ordercloud/angular-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';

export interface IUserPermissionsService {
  getUserGroups(orgID: string, options: ListArgs): Promise<ListPage<UserGroup>>;
  listUserAssignments(userID: string, orgID: string): Promise<ListPage<UserGroupAssignment>>;
  getParentResourceID(): Promise<string>;
  updateUserUserGroupAssignments(
    orgID: string,
    add: UserGroupAssignment[],
    del: UserGroupAssignment[],
    shouldSyncUserCatalogAssignments: boolean
  ): Promise<void>;
}
