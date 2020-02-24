import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListSupplier } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { SupplierCategoryConfig } from '../shopper-context';
import { MiddlewareApiService } from '../services/middleware-api/middleware-api.service';

@Component({
  template: `
    <ocm-supplier-list [suppliers]="suppliers" [supplierCategoryConfig]="supplierCategoryConfig"></ocm-supplier-list>
  `,
})
export class SupplierListWrapperComponent implements OnInit, OnDestroy {
  suppliers: ListSupplier;
  supplierCategoryConfig: SupplierCategoryConfig;
  alive = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    private middleware: MiddlewareApiService,
    public context: ShopperContextService
  ) {}

  ngOnInit(): void {
    this.suppliers = this.activatedRoute.snapshot.data.products;
    this.getSupplierCategories();
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  ngOnDestroy(): void {
    this.alive = false;
  }

  private handleFiltersChange = async (): Promise<void> => {
    this.suppliers = await this.context.supplierFilters.listSuppliers();
  };

  private getSupplierCategories = async (): Promise<void> => {
    this.supplierCategoryConfig = await this.middleware.getMarketplaceSupplierCategories(
      this.context.appSettings.marketplaceID
    );
  };
}
