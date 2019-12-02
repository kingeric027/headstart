import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListSupplier } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { SupplierCategoryConfig } from '../shopper-context';
import { MarketplaceMiddlewareApiService } from '../services/marketplace-middleware-api/marketplace-middleware-api.service';

@Component({
  template: `
    <ocm-supplier-list
      [suppliers]="suppliers"
      [supplierCategoryConfig]="supplierCategoryConfig"
    ></ocm-supplier-list>
  `
})
export class SupplierListWrapperComponent implements OnInit, OnDestroy {
  suppliers: ListSupplier;
  supplierCategoryConfig: SupplierCategoryConfig;
  alive = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    private marketplaceMiddlewareApiService: MarketplaceMiddlewareApiService,
    public context: ShopperContextService
  ) {}

  ngOnInit() {
    this.suppliers = this.activatedRoute.snapshot.data.products;
    this.getSupplierCategories();
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  private handleFiltersChange = async () => {
    this.suppliers = await this.context.supplierFilters.listSuppliers();
  }

  private getSupplierCategories = async () => {
    this.supplierCategoryConfig = await this.marketplaceMiddlewareApiService.getMarketplaceSupplierCategories(
      this.context.appSettings.marketplaceID
    );
  }
  ngOnDestroy() {
    this.alive = false;
  }
}
