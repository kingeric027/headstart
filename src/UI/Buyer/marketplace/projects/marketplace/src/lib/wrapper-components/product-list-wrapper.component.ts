import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { takeWhile } from 'rxjs/operators';
import { MarketplaceMeProduct } from '../shopper-context';
import { ListPage } from 'ordercloud-javascript-sdk';

@Component({
  template: `
    <ocm-product-list [products]="products"></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit, OnDestroy {
  products: ListPage<MarketplaceMeProduct>;
  alive = true;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, public context: ShopperContextService) { }

  ngOnInit(): void {
    this.products = this.activatedRoute.snapshot.data.products;
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

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

  private handleFiltersChange = async (): Promise<void> => {
    const user = this.context.currentUser.get();
    if (user?.UserGroups?.length) {
      this.products = await this.context.productFilters.listProducts();
    } else {
      this.products = {
        Meta: {
          Page: 1,
          PageSize: 20,
          TotalCount: 0,
          TotalPages: 0,
          ItemRange: [
            1,
            0
          ]
        }, Items: []
      }
    };
  }
}
