import { Component, ChangeDetectorRef, NgZone, OnInit } from '@angular/core';
import { Product, OcSupplierService } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { ProductService } from '@app-seller/products/product.service';

@Component({
  selector: 'app-product-table',
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.scss'],
})
export class ProductTableComponent extends ResourceCrudComponent<Product> implements OnInit {
  userContext: {};
  filterConfig: {};
  constructor(
    private productService: ProductService,
    private currentUserService: CurrentUserService,
    private ocSupplierService: OcSupplierService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone,
  ) {
    super(changeDetectorRef, productService, router, activatedRoute, ngZone);
    this.getUserContext(currentUserService);
    this.buildFilterConfig()
  }

  async getUserContext(currentUserService: CurrentUserService): Promise<void> {
    this.userContext = await currentUserService.getUserContext();
  }


  async buildFilterConfig(): Promise<void> {
    const suppliers = await this.ocSupplierService.List({ filters: {Active: 'true'}}).toPromise();
    const supplierFilterOptions = suppliers.Items.map(s => {
      return { Text: s.Name, Value: s.ID}
    })
    this.filterConfig = {
      Filters: [
        {
          Display: 'Status',
          Path: 'xp.Status',
          Items: [
            {Text: 'Draft', Value: 'Draft'},
            {Text: 'Published', Value: 'Published'}],
          Type: 'Dropdown',
        },
        {
          Display: 'Supplier',
          Path: 'DefaultSupplierID',
          Items: supplierFilterOptions,
          Type: 'Dropdown'
        }
      ]
    }
  }

  // static filters that should apply to all marketplace orgs, custom filters for specific applications can be
  // added to the filterconfig passed into the resourcetable in the future
  // filterConfig = {
  //   Filters: [
  //     {
  //       Display: 'Status',
  //       Path: 'xp.Status',
  //       Values: ['Draft', 'Published'],
  //       Type: 'Dropdown',
  //     },
  //   ],
  // };
}
