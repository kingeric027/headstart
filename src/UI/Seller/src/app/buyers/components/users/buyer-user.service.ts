import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { User, OcUserService, UserGroupAssignment, OcUserGroupService } from '@ordercloud/angular-sdk';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';

// TODO - this service is only relevent if you're already on the buyer details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerUserService extends ResourceCrudService<User> {
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

  async updateBuyerUserUserGroupAssignments(
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
}
