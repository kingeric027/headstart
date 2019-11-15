import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ListBuyerProduct, ListCategory, OcMeService } from '@ordercloud/angular-sdk';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { takeWhile } from 'rxjs/operators';

@Component({
  template: `
    <ocm-product-list
      [products]="products"
      [categories]="categories"
      [context]="context"
    ></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit, OnDestroy {
  products: ListBuyerProduct;
  categories: ListCategory;
  alive = true;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService,
  ) {}

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.categories = this.activatedRoute.snapshot.data.categories;
    this.context.productFilters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe(this.handleFiltersChange);
  }

  private handleFiltersChange = async () => {
    this.products = await this.context.productFilters.listProducts();
  }

  configureRouter() {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.router.navigated = false; // TODO - what exactly does this line acomplish?
        // window.scrollTo(0, 0); // scroll to top of screen when new facets are selected.
      }
    });
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
