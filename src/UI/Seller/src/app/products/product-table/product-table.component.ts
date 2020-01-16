import { Component, ChangeDetectorRef, NgZone, OnInit } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-table',
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.scss'],
})
export class ProductTableComponent extends ResourceCrudComponent<Product> implements OnInit {
  constructor(
    private productService: ProductService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, productService, router, activatedRoute, ngZone);
  }
}
