import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, Inject, ChangeDetectorRef } from '@angular/core';
import { User, UserGroup, UserGroupAssignment, OcSupplierUserGroupService, OcSupplierUserService, OcTokenService, ListPage } from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { IUserPermissionsService } from '@app-seller/shared/models/user-permissions.interface';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { GetDisplayText } from './user-group-assignments.constants';
import { Router } from '@angular/router';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { MarketplaceLocationUserGroup } from '@ordercloud/headstart-sdk';
import { List } from 'lodash';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';

interface AssignmentsToAddUpdate {
  UserGroupType: string;
  Assignments: UserGroupAssignment[];
}

@Component({
  selector: 'user-group-assignments',
  templateUrl: './user-group-assignments.component.html',
  styleUrls: ['./user-group-assignments.component.scss'],
})
export class UserGroupAssignments implements OnChanges {
  @Input() user: User;
  @Input() userGroupType: string;
  @Input() isCreatingNew: boolean;
  @Input() userPermissionsService: IUserPermissionsService;
  @Output() assignmentsToAdd = new EventEmitter<AssignmentsToAddUpdate>();
  @Output() hasAssignments = new EventEmitter<boolean>();

  userOrgID: string;
  userID: string;
  userGroups: ListPage<MarketplaceLocationUserGroup> | UserGroup[];
  add: UserGroupAssignment[];
  del: UserGroupAssignment[];
  _userUserGroupAssignmentsStatic: UserGroupAssignment[] = [];
  _userUserGroupAssignmentsEditable: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation = false;
  faExclamationCircle = faExclamationCircle;
  options = {filters: { 'xp.Type': ''}};
  displayText = '';
  searchTermInput: string;
  args: ListArgs = { pageSize: 100 };
  viewAssignedUserGroups = false;
  retrievingAssignments: boolean;

  constructor(
    private http: HttpClient,
    private ocTokenService: OcTokenService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}
  
  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    this.updateForUserGroupAssignmentType();
    this.userOrgID = await this.userPermissionsService.getParentResourceID();
      await this.getUserGroups(this.userOrgID);
    if (changes.user?.currentValue.ID && !this.userID) {
      this.userID = this.user.ID
      if(this.userOrgID && this.userOrgID !== REDIRECT_TO_FIRST_PARENT){
        this.getUserGroupAssignments(this.user.ID, this.userOrgID);
      }
    }
    if (this.userID && changes.user?.currentValue?.ID !== changes.user?.previousValue?.ID) {
      this.userID = this.user.ID
      this.getUserGroupAssignments(this.user.ID, this.userOrgID);
    }
  }

  updateForUserGroupAssignmentType() {
    this.options.filters['xp.Type'] = this.userGroupType;
    this.displayText = GetDisplayText(this.userGroupType);
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getUserGroups(ID: string): Promise<void> {
    if (this.user.xp?.Country) {
      const groups = await this.getUserGroupsByCountry(this.userOrgID, this.user.ID, this.viewAssignedUserGroups);
      this.userGroups = groups;
    } else {
      this.userGroups = await this.userPermissionsService.getUserGroups(ID, this.options);
    }
  }

  async getUserGroupAssignments(userID: any, userOrgID: any): Promise<void> {
    const userGroupAssignments = await this.userPermissionsService.listUserAssignments(userID, userOrgID);
    this._userUserGroupAssignmentsStatic = userGroupAssignments.Items;
    this._userUserGroupAssignmentsEditable = userGroupAssignments.Items;
    const match = this._userUserGroupAssignmentsStatic.some(assignedUG => (this.userGroups as any).Items?.find(ug => ug.ID === assignedUG.UserGroupID));
    this.hasAssignments.emit(match);
  }

  toggleUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newUserUserGroupAssignment = {
        UserID: this.userID,
        UserGroupID: userGroup.ID
      };
      this._userUserGroupAssignmentsEditable = [
        ...this._userUserGroupAssignmentsEditable,
        newUserUserGroupAssignment,
      ];
    }
    this.checkForUserUserGroupAssignmentChanges();
  }

  addUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newUserUserGroupAssignment = {
        UserID: 'PENDING',
        UserGroupID: userGroup.ID
      };
      this._userUserGroupAssignmentsEditable = [
        ...this._userUserGroupAssignmentsEditable,
        newUserUserGroupAssignment,
      ];
    }
    this.checkForUserUserGroupAssignmentChanges();
  }

  isAssigned(userGroup: UserGroup) {
    return (
      this._userUserGroupAssignmentsEditable?.some(
        groupAssignment => groupAssignment.UserGroupID === userGroup.ID
      )
    );
  }

  checkForUserUserGroupAssignmentChanges() {
    this.add = this._userUserGroupAssignmentsEditable.filter(
      editableAssignment => !this._userUserGroupAssignmentsStatic.some(staticAssignment => staticAssignment.UserGroupID === editableAssignment.UserGroupID)
    );
    this.del = this._userUserGroupAssignmentsStatic.filter(
      staticAssignment => !this._userUserGroupAssignmentsEditable.some(editableAssignment => editableAssignment.UserGroupID === staticAssignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
    if (this.isCreatingNew) this.assignmentsToAdd.emit({ UserGroupType: this.userGroupType, Assignments: this.add});
  }

  discardUserUserGroupAssignmentChanges() {
    this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsStatic;
    this.checkForUserUserGroupAssignmentChanges();
  }

  async executeUserUserGroupAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.userPermissionsService.updateUserUserGroupAssignments(this.userOrgID, this.add, this.del, this.userGroupType === "BuyerLocation");
    await this.getUserGroupAssignments(this.userID, this.userOrgID);
    this.checkForUserUserGroupAssignmentChanges();
  }

  async getUserGroupsByCountry(buyerID: string, userID: string, viewAssigned: boolean): Promise<ListPage<MarketplaceLocationUserGroup>> {
    const url = `${this.appConfig.middlewareUrl}/buyerlocations/${buyerID}/usergroups/${userID}/${viewAssigned}`;
    return await this.http.get<ListPage<MarketplaceLocationUserGroup>>(url, { headers: this.buildHeaders(), params: this.createHttpParams(this.args) }).toPromise();
  }

  private buildHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
  }

  private createHttpParams(args: ListArgs): HttpParams {
    let params = new HttpParams();
    Object.entries(args).forEach(([key, value]) => {
      if (key !== 'filters' && value) {
        params = params.append(key, value.toString());
      }
    });
    return params;
  }

  async changePage(page: number) {
    this.args = { ...this.args, page };
    this.userGroups = await this.getUserGroupsByCountry(this.userOrgID, this.user.ID, this.viewAssignedUserGroups);
  }

  async searchedResources(searchText: any) {
    this.searchTermInput = searchText;
    this.args = {...this.args, search: searchText, page: 1 };
    this.userGroups = await this.getUserGroupsByCountry(this.userOrgID, this.user.ID, this.viewAssignedUserGroups);
}

  async toggleUserGroupAssignmentView(value: boolean): Promise<void> {
    this.viewAssignedUserGroups = value;
    this.userGroups = [];
    this.args = { pageSize: 100 };
    this.retrievingAssignments = true;
    this.userGroups = await this.getUserGroupsByCountry(this.userOrgID, this.user.ID, this.viewAssignedUserGroups);
    this.retrievingAssignments = false;
  }
}
