import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  OcAddressService,
  BuyerAddress,
  OcUserGroupService,
  UserGroupAssignment,
  User,
  OcUserService,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
import { BuyerUserService } from '../users/buyer-user.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

export interface PermissionType {
  UserGroupSuffix: string;
  DisplayText: string;
}

export const PermissionTypes: PermissionType[] = [
  { UserGroupSuffix: 'PermissionAdmin', DisplayText: 'Permission Admin' },
  { UserGroupSuffix: 'ResaleCertAdmin', DisplayText: 'Resale Cert Admin' },
  { UserGroupSuffix: 'OrderApprover', DisplayText: 'Order Approver' },
  { UserGroupSuffix: 'NeedsApproval', DisplayText: 'Needs Approval' },
  { UserGroupSuffix: 'ViewAllOrders', DisplayText: 'View All Orders' },
  { UserGroupSuffix: 'CreditCardAdmin', DisplayText: 'Credit Card Admin' },
  { UserGroupSuffix: 'AddressAdmin', DisplayText: 'Address Admin' },
];

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerLocationService extends ResourceCrudService<BuyerAddress> {
  emptyResource = {
    UserGroup: {
      xp: {
        Type: '',
        Currency: null,
      },
      ID: '',
      Name: '',
      Description: '',
    },
    Address: {
      xp: {
        Email: '',
      },
      ID: '',
      DateCreated: '',
      CompanyName: '',
      FirstName: '',
      LastName: '',
      Street1: '',
      Street2: '',
      City: '',
      State: '',
      Zip: '',
      Country: '',
      Phone: '',
      AddressName: '',
    },
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocAddressService: OcAddressService,
    private ocUserGroupService: OcUserGroupService,
    private ocUserService: OcUserService,
    private buyerUserService: BuyerUserService,
    public currentUserService: CurrentUserService
  ) {
    super(router, activatedRoute, ocAddressService, currentUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'locations');
  }

  async updateResource(originalID: string, resource: any): Promise<any> {
    const resourceID = await this.getParentResourceID();
    const newResource = await MarketplaceSDK.ValidatedAddresses.SaveBuyerAddress(
      resourceID,
      originalID,
      resource
    );
    const resourceIndex = this.resourceSubject.value.Items.findIndex((i: any) => i.ID === newResource.ID);
    this.resourceSubject.value.Items[resourceIndex] = newResource;
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  async createNewResource(resource: any): Promise<any> {
    const resourceID = await this.getParentResourceID();
    const newResource = await MarketplaceSDK.ValidatedAddresses.CreateBuyerAddress(
      resourceID,
      resource
    );
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newResource];
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  async getLocationPermissions(locationID: string): Promise<UserGroupAssignment[]> {
    const buyerID = locationID.split('-')[0];
    const requests = PermissionTypes.map(p =>
      // todo accomodate over 100 users
      this.ocUserGroupService
        .ListUserAssignments(buyerID, { userGroupID: `${locationID}-${p.UserGroupSuffix}`, pageSize: 100 })
        .toPromise()
    );
    const responses = await Promise.all(requests);
    return responses.reduce((acc, value) => acc.concat(value.Items), []);
  }

  async getLocationUsers(locationID: string): Promise<User[]> {
    const buyerID = locationID.split('-')[0];
    const userResponse = await this.ocUserService.List(buyerID, { userGroupID: locationID }).toPromise();
    return userResponse.Items;
  }
}
