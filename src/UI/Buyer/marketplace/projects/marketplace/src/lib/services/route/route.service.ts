import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { filter, map } from 'rxjs/operators';
import { ProductFilters, OrderFilters, SupplierFilters, OrderStatus, OrderContext } from '../../shopper-context';
import { OrderFilterService } from '../order-history/order-filter.service';
import { SupplierFilterService } from '../supplier-filter/supplier-filter.service';
import { AuthService } from '../auth/auth.service';
import { ProfileSection, ProfileSections } from './profile-routing.config';
import { TokenHelperService } from '../token-helper/token-helper.service';
export interface IRouter {
  getActiveUrl(): string;
  getOrderContext(): OrderContext;
  onUrlChange(callback: (path: string) => void): void;
  toProductDetails(productID: string): void;
  toProductList(options?: ProductFilters): void;
  toSupplierList(options?: SupplierFilters): void;
  toCheckout(): void;
  toHome(): void;
  toCart(): void;
  toLogin(): void;
  toRegister(): void;
  toForgotPassword(): void;
  toMyProfile(): void;
  toMyAddresses(): void;
  toMyLocations(): void;
  toUsers(): void;
  toMyPaymentMethods(): void;
  toMyOrders(): void;
  toMyOrderDetails(orderID: string): void;
  toOrdersToApprove(): void;
  toOrderToAppoveDetails(orderID: string): void;
  toChangePassword(): void;
  getProfileSections(): ProfileSection[];
  toRoute(path: string): void;
}

@Injectable({
  providedIn: 'root',
})
export class RouteService implements IRouter {
  constructor(
    private router: Router,
    private supplierFilterService: SupplierFilterService,
    private productFilterService: ProductFilterService,
    private orderFilterService: OrderFilterService,
    private tokenHelperService: TokenHelperService
  ) {}

  getActiveUrl(): string {
    return this.router.url;
  }

  getProfileSections(): ProfileSection[] {
    var allSections = ProfileSections;
    var roles = this.tokenHelperService.getDecodedOCToken().role;
    return allSections.filter(
      s => !s.rolesWithAccess || !s.rolesWithAccess.length || roles.some(r => s.rolesWithAccess.includes(r))
    );
  }

  onUrlChange(callback: (path: string) => void): void {
    this.router.events
      .pipe(
        filter(e => e instanceof NavigationEnd),
        map(e => (e as any).url)
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

  toUsers() {
    this.toRoute('/profile/users');
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

  toMyLocations(): void {
    this.toRoute('/profile/locations');
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
    this.router.navigate(['/profile/orders'], { queryParams });
  }

  getOrderContext(): OrderContext {
    if (this.router.url.includes('/approve')) {
      return OrderContext.Approve;
    }
    if (this.router.url.includes('/getapproval')) {
      return OrderContext.GetApproval;
    }
    return OrderContext.Submitted;
  }

  toMyOrderDetails(orderID: string): void {
    this.toRoute(`/profile/orders/${orderID}`);
  }

  toOrdersToApprove(options: OrderFilters = {}): void {
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options);
    this.router.navigate(['/profile/orders/approve'], { queryParams });
  }

  toOrderToAppoveDetails(orderID: string): void {
    this.toRoute(`/profile/orders/approve/${orderID}`);
  }

  toOrdersToGetApproval(options: OrderFilters = {}): void {
    this.toRoute(`/profile/orders/getapproval`);
  }

  toOrderGetApprovalDetails(orderID: string): void {
    this.toRoute(`/profile/orders/getapproval/${orderID}`);
  }

  toSupplierList(options: SupplierFilters = {}): void {
    const queryParams = this.productFilterService.mapToUrlQueryParams(options);
    this.router.navigate(['/suppliers'], { queryParams });
  }

  toChangePassword(): void {
    this.toRoute('/profile/change-password');
  }

  toRoute(path: string): void {
    this.router.navigateByUrl(path);
  }
}
