import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ListSupplier } from "@ordercloud/angular-sdk";
import { takeWhile } from "rxjs/operators";
import { ShopperContextService } from "../services/shopper-context/shopper-context.service";
import { SupplierCategoryConfig } from "../shopper-context";

@Component({
  template: `
    <ocm-supplier-list
      [suppliers]="suppliers"
      [supplierCategoryConfig]="supplierCategoryConfig"
      [context]="context"
    ></ocm-supplier-list>
  `
})
export class SupplierListWrapperComponent implements OnInit, OnDestroy {
  suppliers: ListSupplier;
  supplierCategoryConfig: SupplierCategoryConfig;
  alive = true;

  constructor(
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService
  ) {}

  ngOnInit() {
    this.suppliers = this.activatedRoute.snapshot.data.products;
    this.supplierCategoryConfig = this.context.supplierFilters.getMarketplaceSupplierCategories(
      this.context.appSettings.marketplaceID
    );
    this.context.supplierFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  private handleFiltersChange = async () => {
    this.suppliers = await this.context.supplierFilters.listSuppliers();
  };

  ngOnDestroy() {
    this.alive = false;
  }
}
