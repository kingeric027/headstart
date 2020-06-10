import { Injectable } from '@angular/core';
import {
  OcUserService,
  UserGroup,
  OcUserGroupService,
  UserGroupAssignment,
  OcMeService,
  OcApprovalRuleService,
  OcTokenService,
} from '@ordercloud/angular-sdk';
import { CurrentUserService } from '../current-user/current-user.service';
import { AppConfig } from '../../shopper-context';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MarketplaceUser, ListPage } from 'marketplace-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class UserManagementService {
  constructor(
    private ocUserGroupService: OcUserGroupService,
    private ocMeService: OcMeService,
    public currentUserService: CurrentUserService,
    public ocApprovalRuleService: OcApprovalRuleService,
    private ocUserService: OcUserService,

    // remove below when sdk is regenerated
    private ocTokenService: OcTokenService,
    private httpClient: HttpClient,
    private appConfig: AppConfig
  ) {}

  async getLocations(): Promise<UserGroup[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const loctions = await this.ocMeService
      .ListUserGroups({ pageSize: 100, filters: { 'xp.Type': 'BuyerLocation' } })
      .toPromise();
    return loctions.Items;
  }

  async getLocationUsers(locationID: string): Promise<ListPage<MarketplaceUser>> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/users`;
    return this.httpClient
      .get<ListPage<MarketplaceUser>>(url, { headers })
      .toPromise();
  }

  async getLocationPermissions(locationID: string): Promise<UserGroupAssignment[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const buyerID = locationID.split('-')[0];
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/permissions`;
    return this.httpClient
      .get<UserGroupAssignment[]>(url, { headers })
      .toPromise();
  }

  async getLocationApprovalPermissions(locationID: string): Promise<UserGroupAssignment[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const buyerID = locationID.split('-')[0];
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/approvalpermissions`;
    return this.httpClient
      .get<UserGroupAssignment[]>(url, { headers })
      .toPromise();
  }

  async updateUserUserGroupAssignments(
    buyerID: string,
    locationID: string,
    add: UserGroupAssignment[],
    del: UserGroupAssignment[]
  ): Promise<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const body = {
      AssignmentsToAdd: add,
      AssignmentsToDelete: del,
    };
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/permissions`;
    return this.httpClient
      .post<void>(url, body, { headers })
      .toPromise();
  }

  async getLocationApprovalThreshold(locationID: string): Promise<number> {
    const buyerID = this.currentUserService.get().Buyer.ID;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/approvalthreshold`;
    return this.httpClient
      .get<number>(url, { headers })
      .toPromise();
  }

  async setLocationApprovalThreshold(locationID: string, amount: number): Promise<number> {
    const buyerID = this.currentUserService.get().Buyer.ID;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const body = {
      Threshold: amount,
    };
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/${locationID}/approvalthreshold`;
    return this.httpClient
      .post<number>(url, body, { headers })
      .toPromise();
  }
}
