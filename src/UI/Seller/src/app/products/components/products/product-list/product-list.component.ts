import { Component, ChangeDetectorRef } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent extends ResourceCrudComponent<Product> {
  constructor(private productService: ProductService, changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef, productService);
  }
}
