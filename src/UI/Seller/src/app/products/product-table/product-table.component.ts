import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { Product } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

function createProductForm(product: Product) {
  return new FormGroup({
    Name: new FormControl(product.Name, Validators.required),
    Description: new FormControl(product.Description),
    Inventory: new FormControl(product.Inventory),
    QuantityMultiplier: new FormControl(product.QuantityMultiplier),
    ShipFromAddressID: new FormControl(product.ShipFromAddressID),
    ShipHeight: new FormControl(product.ShipHeight),
    ShipWidth: new FormControl(product.ShipWidth),
    ShipLength: new FormControl(product.ShipLength),
    ShipWeight: new FormControl(product.ShipWeight),
    // SpecCount: new FormControl(product.SpecCount),
    // VariantCount: new FormControl(product.VariantCount),
    xp: new FormControl(product.xp),
  });
}
@Component({
  selector: 'app-product-table',
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.scss'],
})
export class ProductTableComponent extends ResourceCrudComponent<Product> {
  constructor(
    private productService: ProductService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, productService, router, activatedRoute, ngZone, createProductForm);
  }
}
