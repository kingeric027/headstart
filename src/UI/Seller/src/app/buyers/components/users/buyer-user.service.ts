import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import {
  User,
  OcUserService,
  UserGroupAssignment,
  OcUserGroupService,
  ListUserGroup,
  ListUserGroupAssignment,
} from '@ordercloud/angular-sdk';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';
import { IUserPermissionsService } from '@app-seller/shared/models/user-permissions.interface';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';

// TODO - this service is only relevent if you're already on the buyer details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerUserService extends ResourceCrudService<User> implements IUserPermissionsService {
  emptyResource = {
    Username: '',
    FirstName: '',
    LastName: '',
    Email: '',
    Phone: '',
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocUserService: OcUserService,
    private ocBuyerUserGroupService: OcUserGroupService
  ) {
    super(router, activatedRoute, ocUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'users');
  }

  async updateUserUserGroupAssignments(
    buyerID: string,
    add: UserGroupAssignment[],
    del: UserGroupAssignment[]
  ): Promise<void> {
    const addRequests = add.map(newAssignment => this.addBuyerUserUserGroupAssignment(buyerID, newAssignment));
    const deleteRequests = del.map(assignmentToRemove =>
      this.removeBuyerUserUserGroupAssignment(buyerID, assignmentToRemove)
    );
    await Promise.all([...addRequests, ...deleteRequests]);
  }

  addBuyerUserUserGroupAssignment(buyerID: string, assignment: UserGroupAssignment): Promise<void> {
    return this.ocBuyerUserGroupService
      .SaveUserAssignment(buyerID, { UserID: assignment.UserID, UserGroupID: assignment.UserGroupID })
      .toPromise();
  }

  removeBuyerUserUserGroupAssignment(buyerID: string, assignment: UserGroupAssignment): Promise<void> {
    return this.ocBuyerUserGroupService
      .DeleteUserAssignment(buyerID, assignment.UserGroupID, assignment.UserID)
      .toPromise();
  }

  async getUserGroups(buyerID: string, options: ListArgs): Promise<ListUserGroup> {
    return await this.ocBuyerUserGroupService.List(buyerID, options).toPromise();
  }

  async listUserAssignments(userID: string, buyerID: string): Promise<ListUserGroupAssignment> {
    return await this.ocBuyerUserGroupService.ListUserAssignments(buyerID, { userID }).toPromise();
  }
}
