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

  toCart(): void {
    this.router.navigateByUrl('/cart');
  }

  toLogin(): void {
    this.router.navigateByUrl('/login');
  }

  toRegister(): void {
    this.router.navigateByUrl('/register');
  }

  toMyProfile(): void {
    this.router.navigateByUrl('/profile/details');
  }

  toMyAddresses(): void {
    this.router.navigateByUrl('/profile/addresses');
  }

  toMyPaymentMethods(): void {
    this.router.navigateByUrl('/profile/payment-methods');
  }

  toMyOrders(): void {
    this.router.navigateByUrl('/profile/orders');
  }

  toOrdersToApprove(): void {
    this.router.navigateByUrl('/profile/approval');
  }
}
