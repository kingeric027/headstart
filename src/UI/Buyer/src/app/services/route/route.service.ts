import { Injectable } from '@angular/core'
import { Router, NavigationEnd } from '@angular/router'
import { ProductFilterService } from '../product-filter/product-filter.service'
import { filter, map } from 'rxjs/operators'
import {
  ProductFilters,
  OrderFilters,
  SupplierFilters,
  OrderStatus,
  OrderViewContext,
} from '../../shopper-context'
import { OrderFilterService } from '../order-history/order-filter.service'
import { ProfileRoutes } from './profile-routing.config'
import { OrderRoutes } from './order-routing.config'
import { RouteConfig } from './route-config'
import { SupplierFilterService } from '../supplier-filter/supplier-filter.service'
import { TokenHelperService } from '../token-helper/token-helper.service'

@Injectable({
  providedIn: 'root',
})
export class RouteService {
  constructor(
    private router: Router,
    private supplierFilterService: SupplierFilterService,
    private productFilterService: ProductFilterService,
    private orderFilterService: OrderFilterService,
    private tokenHelperService: TokenHelperService
  ) {}

  getActiveUrl(): string {
    return this.router.url
  }

  getProfileRoutes(): RouteConfig[] {
    const allSections = ProfileRoutes
    const roles = this.tokenHelperService.getDecodedOCToken().role
    return allSections.filter(
      (s) =>
        !s.rolesWithAccess ||
        !s.rolesWithAccess.length ||
        roles.some((r) => s.rolesWithAccess.includes(r))
    )
  }

  getOrderRoutes(): RouteConfig[] {
    const allSections = OrderRoutes
    const roles = this.tokenHelperService.getDecodedOCToken().role
    return allSections.filter(
      (s) =>
        !s.rolesWithAccess ||
        !s.rolesWithAccess.length ||
        roles.some((r) => s.rolesWithAccess.includes(r))
    )
  }

  onUrlChange(callback: (path: string) => void): void {
    this.router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        map((e) => (e as any).url)
      )
      .subscribe(callback)
  }

  toProductDetails(productID: string): void {
    this.router.navigateByUrl(`/products/${productID}`)
  }

  toProductList(options: ProductFilters = {}): void {
    const queryParams = this.productFilterService.mapToUrlQueryParams(options)
    this.router.navigate(['/products'], { queryParams })
  }

  toHome(): void {
    this.toRoute('/home')
  }

  toUsers(): void {
    this.toRoute('/profile/users')
  }

  toCheckout(): void {
    this.toRoute('/checkout')
  }

  toCart(): void {
    this.toRoute('/cart')
  }

  toLogin(): void {
    this.toRoute('/login')
  }

  toForgotPassword(): void {
    this.toRoute('/forgot-password')
  }

  toRegister(): void {
    this.toRoute('/register')
  }

  toMyProfile(): void {
    this.router.navigateByUrl('/profile')
  }

  toMyAddresses(): void {
    this.toRoute('/profile/addresses')
  }

  toMyLocations(): void {
    this.toRoute('/profile/locations')
  }

  toLocationManagement(addressID: string): void {
    this.toRoute(`/profile/locations/${addressID}`)
  }

  toMyPaymentMethods(): void {
    this.toRoute('/profile/payment-methods')
  }

  toMyOrders(options: OrderFilters = {}): void {
    // routing directly to unsubmitted orders
    if (!options.status) {
      options.status = OrderStatus.AllSubmitted
    }
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options)
    this.router.navigate(['/orders'], { queryParams })
  }

  toOrdersByLocation(options: OrderFilters = {}): void {
    // routing directly to unsubmitted orders
    if (!options.status) {
      options.status = OrderStatus.AllSubmitted
    }
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options)
    this.router.navigate(['/orders/location'], { queryParams })
  }

  toMyOrderDetails(orderID: string): void {
    this.toRoute(`/orders/${orderID}`)
  }

  toOrdersToApprove(options: OrderFilters = {}): void {
    const queryParams = this.orderFilterService.mapToUrlQueryParams(options)
    this.router.navigate(['/orders/approve'], { queryParams })
  }

  toOrderToAppoveDetails(orderID: string): void {
    this.toRoute(`/orders/approve/${orderID}`)
  }

  toSupplierList(options: SupplierFilters = {}): void {
    const queryParams = this.supplierFilterService.mapToUrlQueryParams(options)
    this.router.navigate(['/suppliers'], { queryParams })
  }

  toChangePassword(): void {
    this.toRoute('/profile/change-password')
  }

  toRoute(path: string): void {
    this.router.navigateByUrl(path)
  }
}