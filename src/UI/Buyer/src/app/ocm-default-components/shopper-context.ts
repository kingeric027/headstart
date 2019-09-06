import { LineItem, MeUser, Order, ListLineItem, User } from '@ordercloud/angular-sdk';
import { Input } from '@angular/core';
import { ProductListParams } from '@app-buyer/shared/services/product-list/product-list.service';

export class OCMComponent {
  @Input() context: ShopperContext;
}

export interface ShopperContext {
  cartActions: CartActions;
  routeActions: RouteActions;
  currentUser: CurrentUser;
  currentOrder: CurrentOrder;
  productListActions: ProductListActions;
}

export interface CartActions {
  addToCart: (lineItem: LineItem) => Promise<LineItem>;
  removeLineItem: (lineItemID: string) => Promise<void>;
  updateQuantity: (lineItemID: string, newQuantity: number) => Promise<LineItem>;
  addManyToCart: (lineItem: LineItem[]) => Promise<LineItem[]>;
  emptyCart: () => Promise<void>;
}

export interface RouteActions {
  toProductDetails: (productID: string) => void;
  toProductList: (options?: ProductListParams) => void;
  toCheckout: () => void;
}

export interface CurrentUser {
  user: MeUser;
  onUserChange: (callback: (user: User) => void) => void;
  onIsAnonymousChange: (callback: (isAnonymous: boolean) => void) => void;
  onFavoriteProductsChange: (callback: (productIDs: string[]) => void) => void;
  setIsFavoriteProduct: (isFav: boolean, productID: string) => void;
  onFavoriteOrdersChange: (callback: (orderIDs: string[]) => void) => void;
  setIsFavoriteOrder: (isFav: boolean, orderID: string) => void;
}

export interface CurrentOrder {
  order: Order;
  lineItems: ListLineItem;
  onOrderChange: (callback: (order: Order) => void) => void;
  onLineItemsChange: (callback: (lineItems: ListLineItem) => void) => void;
}

export interface ProductListActions {
  toPage: (pageNumber: number) => void;
  sortBy: (field: string) => void;
  clearSort: () => void;
  searchBy: (searchTerm: string) => void;
  clearSearch: () => void;
  filterBy: (field: string, value: string, isFacet?: boolean) => void;
  removeFilter: (field: string) => void;
  filterByCategory: (categoryID: string) => void;
  clearCategoryFilter: () => void;
  filterByFavorites: (showOnlyFavorites: boolean) => void;
  clearAllFilters: () => void;
}
