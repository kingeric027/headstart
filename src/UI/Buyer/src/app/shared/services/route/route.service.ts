import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { IRouteActions, ProductFilters } from 'src/app/ocm-default-components/shopper-context';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { filter, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements IRouteActions {
  currentPath: any;

  constructor(private router: Router, private productFilterService: ProductFilterService) {}

  onUrlChange(callback: (path: string) => void): void {
    this.router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        map((e) => (e as any).url)
      )
      .subscribe(callback);
  }

  toProductDetails(productID: string): void {
    this.router.navigateByUrl(`/products/${productID}`);
  }

  toProductList(options: ProductFilters = {}): void {
    const queryParams = this.productFilterService.mapToUrlQueryParams(options);
    this.router.navigate(['/products'], { queryParams });
  }

  toHome() {
    this.router.navigateByUrl('/home');
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

  toOrderDetails(orderID: string): void {
    this.router.navigateByUrl(`/profile/orders/${orderID}`);
  }

  toMyOrders(): void {
    this.router.navigateByUrl(`/profile/orders`);
  }

  toOrdersToApprove(): void {
    this.router.navigateByUrl('/profile/orders/approval');
  }
}
