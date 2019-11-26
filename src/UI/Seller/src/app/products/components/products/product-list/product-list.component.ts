import { Component, ChangeDetectorRef } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent extends ResourceCrudComponent<Product> {
  constructor(
    private productService: ProductService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute
  ) {
    super(changeDetectorRef, productService, router, activatedRoute);
  }
}
