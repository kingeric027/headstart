import { Component, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { ListLineItem, Order } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { IShopperContext } from '@app-buyer/ocm-default-components/shopper-context';

@Component({
  selector: 'ocm-mini-cart',
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart implements OnChanges {
  @Input() context: IShopperContext;
  @Output() navigate = new EventEmitter();
  lineItems: ListLineItem;
  order: Order;
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  constructor() {}

  ngOnChanges() {
    this.context.currentOrder.onLineItemsChange((lis) => (this.lineItems = lis));
    this.context.currentOrder.onOrderChange((order) => (this.order = order));
  }

  toFullCart() {
    this.context.routeActions.toCart();
    this.navigate.emit();
  }

  toProductDetails(productID: string) {
    this.context.routeActions.toProductDetails(productID);
    this.navigate.emit();
  }

  toCheckout() {
    this.context.routeActions.toCheckout();
    this.navigate.emit();
  }
}
