import { LineItem, MeUser, Order, ListLineItem, User, AccessToken } from '@ordercloud/angular-sdk';
import { Input } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '../config/app.config';

export class OCMComponent {
  // todo: the issue is that ngOnInit fires before inputs are ready. come up with a better way to do this.
  observersSet: boolean;
  @Input() context: IShopperContext;
}

export interface IShopperContext {
  cartActions: ICartActions;
  routeActions: IRouteActions;
  currentUser: ICurrentUser;
  currentOrder: ICurrentOrder;
  productFilterActions: IProductFilterActions;
  authentication: IAuthActions;
  appSettings: AppConfig; // TODO - should this come from custom-components repo somehow? Or be configured in admin and persisted in db?
}

export interface ICartActions {
  addToCartSubject: Observable<LineItem>;
  addToCart(lineItem: LineItem): Promise<LineItem>;
  removeLineItem(lineItemID: string): Promise<void>;
  updateQuantity(lineItemID: string, newQuantity: number): Promise<LineItem>;
  addManyToCart(lineItem: LineItem[]): Promise<LineItem[]>;
  emptyCart(): Promise<void>;
  onAddToCart(callback: (lineItem: LineItem) => void): void;
}

export interface IRouteActions {
  onUrlChange(callback: (path: string) => void): void;
  toProductDetails(productID: string): void;
  toProductList(options?: ProductFilters): void;
  toCheckout(): void;
  toHome(): void;
  toCart(): void;
  toLogin(): void;
  toRegister(): void;
  toForgotPassword(): void;
  toMyProfile(): void;
  toMyAddresses(): void;
  toMyPaymentMethods(): void;
  toMyOrders(): void;
  toOrdersToApprove(): void;
  toOrderDetails(orderID: string): void;
  toOrderConfirmation(orderID: string): void;
}

export interface ICurrentUser {
  favoriteProductIDs: string[];
  favoriteOrderIDs: string[];
  isAnonymous: boolean;
  get(): MeUser;
  patch(user: MeUser): void;
  onUserChange(callback: (user: User) => void): void;
  onIsAnonymousChange(callback: (isAnonymous: boolean) => void): void;
  onFavoriteProductsChange(callback: (productIDs: string[]) => void): void;
  setIsFavoriteProduct(isFav: boolean, productID: string): void;
  onFavoriteOrdersChange(callback: (orderIDs: string[]) => void): void;
  setIsFavoriteOrder(isFav: boolean, orderID: string): void;
}

export interface ICurrentOrder {
  order: Order;
  lineItems: ListLineItem;
  onOrderChange(callback: (order: Order) => void): void;
  onLineItemsChange(callback: (lineItems: ListLineItem) => void): void;
}

export interface IProductFilterActions {
  toPage(pageNumber: number): void;
  sortBy(field: string): void;
  clearSort(): void;
  searchBy(searchTerm: string): void;
  clearSearch(): void;
  filterByFacet(field: string, value: string, isFacet?: boolean): void;
  clearFacetFilter(field: string): void;
  filterByCategory(categoryID: string): void;
  clearCategoryFilter(): void;
  filterByFavorites(showOnlyFavorites: boolean): void;
  clearAllFilters(): void;
  onFiltersChange(callback: (filters: ProductFilters) => void): void;
}

export interface IAuthActions {
  profiledLogin(username: string, password: string): Promise<AccessToken>;
  logout(): Promise<void>;
  changePassword(newPassword: string): Promise<void>;
  anonymousLogin(): Promise<AccessToken>;
  getOrderCloudToken(): string;
}

export interface ProductFilters {
  page?: number;
  sortBy?: string;
  search?: string;
  categoryID?: string;
  showOnlyFavorites?: boolean;
  activeFacets?: any;
}
