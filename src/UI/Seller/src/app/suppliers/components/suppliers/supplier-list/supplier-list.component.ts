import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Supplier } from '@ordercloud/angular-sdk';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';

@Component({
  selector: 'app-supplier-list',
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss'],
})
export class SupplierListComponent extends ResourceCrudComponent<Supplier> {
  constructor(private supplierService: SupplierService, changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef, supplierService);
  }
}
