import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { IRouteActions, ProductFilters } from '@app-buyer/ocm-default-components/shopper-context';
import { ProductFilterService } from '../product-filter/product-filter.service';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements IRouteActions {
  constructor(private router: Router, private productFilterService: ProductFilterService) {}

  toProductDetails(productID: string): void {
    this.router.navigateByUrl(`/products/${productID}`);
  }

  toProductList(options: ProductFilters = null): void {
    const queryParams = this.productFilterService.mapToUrlQueryParams(options);
    this.router.navigate(['/products'], { queryParams });
  }

  toCheckout(): void {
    this.router.navigateByUrl('/checkout');
  }
}
