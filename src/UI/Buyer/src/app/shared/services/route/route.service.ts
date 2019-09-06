import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { RouteActions } from '@app-buyer/ocm-default-components/shopper-context';
import { ProductListParams, ProductListService } from '../product-list/product-list.service';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements RouteActions {
  constructor(private router: Router, private productListService: ProductListService) {}

  toProductDetails(productID: string): void {
    this.router.navigateByUrl(`/products/${productID}`);
  }

  toProductList(options: ProductListParams = null): void {
    const queryParams = this.productListService.mapToUrlQueryParams(options);
    this.router.navigate(['/products'], { queryParams });
  }

  toCheckout(): void {
    this.router.navigateByUrl('/checkout');
  }
}
