import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Supplier } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierUserService } from '@app-seller/shared/services/supplier-user/supplier-user.service';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';

@Component({
  selector: 'app-supplier-user-table',
  templateUrl: './supplier-user-table.component.html',
  styleUrls: ['./supplier-user-table.component.scss'],
})
export class SupplierUserTableComponent extends ResourceCrudComponent<Supplier> {
  constructor(
    private supplierUserService: SupplierUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private supplierService: SupplierService
  ) {
    super(changeDetectorRef, supplierUserService, router, activatedroute);
  }
}
