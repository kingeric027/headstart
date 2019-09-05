import { LineItem, MeUser, Order, ListLineItem } from '@ordercloud/angular-sdk';
import { Input } from '@angular/core';

export class OCMComponent {
  @Input() context: ShopperContext;
}

export interface ShopperContext {
  cartActions: CartActions;
  routeActions: RouteActions;
  currentUser: CurrentUser;
  currentOrder: CurrentOrder;
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
  toProductList: () => void;
  toCheckout: () => void;
}

export interface CurrentUser {
  user: MeUser;
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
