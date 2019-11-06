import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { filter, map } from 'rxjs/operators';
import { IRouter, ProductFilters, OrderFilters, OrderStatus } from '../../shopper-context';
import { OrderFilterService } from '../order-history/order-filter.service';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements IRouter {
  constructor(private router: Router, private productFilterService: ProductFilterService, private orderFilterService: OrderFilterService) {}

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

  toMyOrders(options: OrderFilters = {}): void {
    // routing directly to unsubmitted orders
    if (!options.status) {
      options.status = OrderStatus.AllSubmitted;
    }
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options);
    this.router.navigate([`/profile/orders`], { queryParams });
  }

  toMyOrderDetails(orderID: string): void {
    this.toRoute(`/profile/orders/${orderID}`);
  }

  toOrdersToApprove(options: OrderFilters = {}): void {
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options);
    this.router.navigate([`/profile/orders/approval`], { queryParams });
  }

  toOrderToAppoveDetails(orderID: string): void {
    this.toRoute(`/profile/orders/approval/${orderID}`);
  }


  toChangePassword(): void {
    this.toRoute('/profile/change-password');
  }

  toRoute(path: string): void {
    this.router.navigateByUrl(path);
  }
}
