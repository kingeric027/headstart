import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Address } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierUserService } from '@app-seller/shared/services/supplier/supplier-user.service';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';

@Component({
  selector: 'app-supplier-location-table',
  templateUrl: './supplier-location-table.component.html',
  styleUrls: ['./supplier-location-table.component.scss'],
})
export class SupplierLocationTableComponent extends ResourceCrudComponent<Address> {
  constructor(
    private supplierAddressService: SupplierAddressService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private supplierService: SupplierService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, supplierAddressService, router, activatedroute, ngZone);
  }
}