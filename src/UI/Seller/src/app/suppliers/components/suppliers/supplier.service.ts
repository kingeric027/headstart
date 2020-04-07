import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  Supplier,
  OcSupplierService,
  OcMeService,
  UserGroupAssignment,
  OcSupplierUserGroupService,
  ListUserGroup,
  ListUserGroupAssignment,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';

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
    private ocMeService: OcMeService
  ) {
    super(router, activatedRoute, ocSupplierService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST);
    this.ocSupplierService = ocSupplierService;
  }

  async createNewResource(resource: any): Promise<any> {
    resource.ID = '{supplierIncrementor}';
    if (!resource.xp?.Images[0]?.URL) resource.xp.Images = [];
    const newSupplier = await MarketplaceSDK.Suppliers.Create(resource);
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newSupplier];
    this.resourceSubject.next(this.resourceSubject.value);
    return newSupplier;
  }

  async getMyResource(): Promise<any> {
    const me = await this.ocMeService.Get().toPromise();
    const supplier = await this.ocSupplierService.Get(me.Supplier.ID).toPromise();
    return supplier;
  }
}
