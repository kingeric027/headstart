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
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

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
      Currency: null,
      Images: [{ URL: '', Tag: null }],
      SupportContact: { Name: '', Email: '', Phone: '' },
      SyncFrieghtPop: false
    },
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocSupplierService: OcSupplierService,
    private ocSupplierUserGroupService: OcSupplierUserGroupService,
    private ocMeService: OcMeService,
    public currentUserService: CurrentUserService
  ) {
    super(router, activatedRoute, ocSupplierService, currentUserService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST);
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
}
