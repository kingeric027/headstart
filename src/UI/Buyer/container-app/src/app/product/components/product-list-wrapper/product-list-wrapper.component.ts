import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ListBuyerProduct, ListCategory, OcMeService } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { BuildQtyLimits } from 'src/app/shared';
import { QuantityLimits } from 'src/app/shared/models/quantity-limits';

@Component({
  template: `
    <ocm-product-list
      [products]="products"
      [categories]="categories"
      [quantityLimits]="quantityLimits"
      [context]="context"
    ></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit {
  products: ListBuyerProduct;
  categories: ListCategory;
  quantityLimits: QuantityLimits[];

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService,
    private ocMeService: OcMeService
  ) {}

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.categories = this.activatedRoute.snapshot.data.categories;
    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
    this.context.productFilters.onFiltersChange(this.handleFiltersChange);
  }

  private handleFiltersChange = async () => {
    const queryParams = this.context.productFilters.getOrderCloudParams();
    this.products = await this.ocMeService.ListProducts(queryParams).toPromise();
    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
  };

  configureRouter() {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.router.navigated = false; // TODO - what exactly does this line acomplish?
        //window.scrollTo(0, 0); // scroll to top of screen when new facets are selected.
      }
    });
  }
}
