import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { takeWhile } from 'rxjs/operators';
import { ListMarketplaceMeProduct } from '../shopper-context';

@Component({
  template: `
    <ocm-product-list [products]="products"></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit, OnDestroy {
  products: ListMarketplaceMeProduct;
  alive = true;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

  ngOnInit(): void {
    this.products = this.activatedRoute.snapshot.data.products;
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  private handleFiltersChange = async (): Promise<void> => {
    this.products = await this.context.productFilters.listProducts();
  };

  configureRouter(): void {
    this.router.events.subscribe(evt => {
      if (evt instanceof NavigationEnd) {
        this.router.navigated = false; // TODO - what exactly does this line acomplish?
        // window.scrollTo(0, 0); // scroll to top of screen when new facets are selected.
      }
    });
  }

  ngOnDestroy(): void {
    this.alive = false;
  }
}
