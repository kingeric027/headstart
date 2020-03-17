import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcSupplierAddressService, Address, ListAddress } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { SUPPLIER_SUB_RESOURCE_LIST } from '../suppliers/supplier.service';

@Injectable({
  providedIn: 'root',
})
export class SupplierAddressService extends ResourceCrudService<Address> {
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    private ocSupplierAddressService: OcSupplierAddressService
  ) {
    super(
      router,
      activatedRoute,
      ocSupplierAddressService,
      '/suppliers',
      'suppliers',
      SUPPLIER_SUB_RESOURCE_LIST,
      'locations'
    );
  }

  async createNewResource(resource: any): Promise<any> {
    // special iding process for supplier addresses
    const parentResourceID = this.getParentResourceID();
    const existingAddresses = await this.ocSupplierAddressService.List(parentResourceID).toPromise();
    const newID = this.getIncrementedID(parentResourceID, existingAddresses);
    resource.ID = newID;

    const newResource = await this.ocService.Create(...this.createListArgs([resource])).toPromise();
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newResource];
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  private getIncrementedID(supplierID: string, existingAddresses: ListAddress): string {
    const numbers = existingAddresses.Items.map(a => Number(a.ID.split('-')[1]));
    const highestNumber = Math.max(...numbers);
    const nextID = highestNumber + 1;
    return `${supplierID}-${nextID.toString().padStart(2, '0')}`;
  }

  emptyResource = {
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
    AddressName: 'Your new supplier location',
    xp: null,
  };
}
