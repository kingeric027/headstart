import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { filter, map } from 'rxjs/operators';
import { IRouter, ProductFilters } from 'shopper-context-interface';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements IRouter {
  constructor(private router: Router, private productFilterService: ProductFilterService) {}

  getActiveUrl(): string {
    return this.router.url;
  }

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
    this.toRoute('/home');
  }

  toCheckout(): void {
    this.toRoute('/checkout');
  }

  toCart(): void {
    this.toRoute('/cart');
  }

  toLogin(): void {
    this.toRoute('/login');
  }

  toForgotPassword(): void {
    this.toRoute('/forgot-password');
  }

  toRegister(): void {
    this.toRoute('/register');
  }

  toMyProfile(): void {
    this.router.navigateByUrl('/profile');
  }

  toMyAddresses(): void {
    this.toRoute('/profile/addresses');
  }

  toMyPaymentMethods(): void {
    this.toRoute('/profile/payment-methods');
  }

  toOrderDetails(orderID: string): void {
    this.toRoute(`/profile/orders/${orderID}`);
  }

  toMyOrders(): void {
    this.toRoute(`/profile/orders`);
  }

  toOrdersToApprove(): void {
    this.toRoute('/profile/orders/approval');
  }

  toChangePassword(): void {
    this.toRoute('/profile/change-password');
  }

  toRoute(path: string): void {
    this.router.navigateByUrl(path);
  }
}
