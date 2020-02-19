import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  Supplier,
  OcSupplierService,
  OcMeService,
  UserGroupAssignment,
  OcSupplierUserGroupService,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';

export const SUPPLIER_SUB_RESOURCE_LIST = ['users', 'locations'];
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SupplierService extends ResourceCrudService<Supplier> {
  ocSupplierService: OcSupplierService;

  emptyResource = {
    Name: '',
    xp: {
      Description: '',
      Images: [{ URL: '', Tag: null }],
      SupportContact: { Name: '', Email: '', Phone: '' },
    },
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocSupplierService: OcSupplierService,
    private ocSupplierUserGroupService: OcSupplierUserGroupService,
    private ocMeService: OcMeService,
    private middleware: MiddlewareAPIService
  ) {
    super(router, activatedRoute, ocSupplierService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST);
    this.ocSupplierService = ocSupplierService;
  }

  async createNewResource(resource: any): Promise<any> {
    const newSupplier = await this.middleware.createSupplier(resource);
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newSupplier];
    this.resourceSubject.next(this.resourceSubject.value);
    return newSupplier;
  }

  async getMyResource(): Promise<any> {
    const me = await this.ocMeService.Get().toPromise();
    const supplier = await this.ocSupplierService.Get(me.Supplier.ID).toPromise();
    return supplier;
  }

  async updateSupplierUserUserGroupAssignments(
    supplierID: string,
    add: UserGroupAssignment[],
    del: UserGroupAssignment[]
  ): Promise<void> {
    const addRequests = add.map(newAssignment => this.addSupplierUserUserGroupAssignment(supplierID, newAssignment));
    const deleteRequests = del.map(assignmentToRemove =>
      this.removeSupplierUserUserGroupAssignment(supplierID, assignmentToRemove)
    );
    await Promise.all([...addRequests, ...deleteRequests]);
  }

  addSupplierUserUserGroupAssignment(supplierID: string, assignment: UserGroupAssignment): Promise<void> {
    return this.ocSupplierUserGroupService
      .SaveUserAssignment(supplierID, { UserID: assignment.UserID, UserGroupID: assignment.UserGroupID })
      .toPromise();
  }

  removeSupplierUserUserGroupAssignment(supplierID: string, assignment: UserGroupAssignment): Promise<void> {
    return this.ocSupplierUserGroupService
      .DeleteUserAssignment(supplierID, assignment.UserGroupID, assignment.UserID)
      .toPromise();
  }
}
