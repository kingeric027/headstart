import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeWhile } from 'rxjs/operators';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { MarketplaceSDK, SupplierCategoryConfig, ListPage } from 'marketplace-javascript-sdk';
import { Supplier } from 'ordercloud-javascript-sdk';

@Component({
  template: `
    <ocm-supplier-list [suppliers]="suppliers" [supplierCategoryConfig]="supplierCategoryConfig"></ocm-supplier-list>
  `,
})
export class SupplierListWrapperComponent implements OnInit, OnDestroy {
  suppliers: ListPage<Supplier>;
  supplierCategoryConfig: SupplierCategoryConfig;
  alive = true;

  constructor(private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

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
    const marketplaceID = this.context.appSettings.marketplaceID;
    this.supplierCategoryConfig = await MarketplaceSDK.SupplierCategoryConfigs.Get(marketplaceID);
  };
}
