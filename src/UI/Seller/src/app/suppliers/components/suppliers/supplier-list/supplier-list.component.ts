import { Component, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { ProductService } from '@app-seller/shared/services/product/product.service';

@Component({
  selector: 'app-supplier-list',
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss'],
})
export class SupplierListComponent extends ResourceCrudComponent<Supplier> {
  constructor(private productService: ProductService, changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef, productService);
  }
}
