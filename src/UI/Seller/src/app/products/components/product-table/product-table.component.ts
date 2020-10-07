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
    this.buildFilterConfig();
  }

  async getUserContext(currentUserService: CurrentUserService): Promise<void> {
    this.userContext = await currentUserService.getUserContext();
  }

  async buildFilterConfig(): Promise<void> {
    let supplierListPage = await this.ocSupplierService
      .List({ pageSize: 100, sortBy: ['Name'], filters: { Active: 'true' }})
      .toPromise();
    let suppliers = supplierListPage.Items;
    if (supplierListPage.Meta.TotalPages > 1) {
      for (let i = 2; i <= supplierListPage.Meta.TotalPages; i++) {
        let newSuppliers = await this.ocSupplierService
          .List({ pageSize: 100, sortBy: ['Name'], filters: { Active: 'true' }, page: i })
          .toPromise();
        suppliers = suppliers.concat(newSuppliers.Items);
      }
    }
    let supplierFilterOptions = suppliers.map(s => {
      return { Text: s.Name, Value: s.ID };
    });
    // static filters that should apply to all marketplace orgs, custom filters for specific applications can be
    // added to the filterconfig passed into the resourcetable in the future
    this.filterConfig = {
      Filters: [
        {
          Display: 'ADMIN.FILTERS.STATUS',
          Path: 'xp.Status',
          Items: [
            { Text: 'ADMIN.FILTER_OPTIONS.DRAFT', Value: 'Draft' },
            { Text: 'ADMIN.FILTER_OPTIONS.PUBLISHED', Value: 'Published' },
          ],
          Type: 'Dropdown',
        },
        {
          Display: 'ADMIN.FILTERS.SUPPLIER',
          Path: 'DefaultSupplierID',
          Items: supplierFilterOptions,
          Type: 'Dropdown'
        }
      ]
    }
  }
}
