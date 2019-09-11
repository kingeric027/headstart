import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ListBuyerProduct, ListCategory, OcMeService } from '@ordercloud/angular-sdk';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'product-list-wrapper',
  templateUrl: './product-list-wrapper.component.html',
  styleUrls: ['./product-list-wrapper.component.scss'],
})
export class ProductListWrapperComponent implements OnInit {
  products: ListBuyerProduct;
  categories: ListCategory;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    protected context: ShopperContextService,
    private ocMeService: OcMeService
  ) {}

  ngOnInit() {
    this.products = this.activatedRoute.snapshot.data.products;
    this.categories = this.activatedRoute.snapshot.data.categories;
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
  }

  private handleFiltersChange = async () => {
    const queryParams = this.context.productFilterActions.getOrderCloudParams();
    this.products = await this.ocMeService.ListProducts(queryParams).toPromise();
  };

  configureRouter() {
    this.router.events.subscribe((evt) => {
      if (evt instanceof NavigationEnd) {
        this.router.navigated = false; // TODO - what exactly does this line acomplish?
        window.scrollTo(0, 0); // scroll to top of screen when new facets are selected.
      }
    });
  }
}
