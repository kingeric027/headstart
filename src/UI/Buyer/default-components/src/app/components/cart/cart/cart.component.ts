import { Component, Input } from '@angular/core';
import { ListLineItemWithProduct, ShopperContextService } from 'marketplace';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';
import { OrderSummaryMeta, getOrderSummaryMeta } from 'src/app/services/purchase-order.helper';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart {
  _order: MarketplaceOrder;
  _lineItems: ListLineItemWithProduct;
  orderSummaryMeta: OrderSummaryMeta;
  @Input() set order(value: MarketplaceOrder) {
    this._order = value;
    this.setOrderSummaryMeta();
  }
  @Input() set lineItems(value: ListLineItemWithProduct) {
    this._lineItems = value;
    this.setOrderSummaryMeta();
  }

  constructor(private context: ShopperContextService) {}

  setOrderSummaryMeta(): void {
    if (this._order && this._lineItems) {
      this.orderSummaryMeta = getOrderSummaryMeta(this._order, this._lineItems.Items, 'cart');
    }
  }
  toProductList(): void {
    this.context.router.toProductList();
  }

  toCheckout(): void {
    this.context.router.toCheckout();
  }

  emptyCart(): void {
    this.context.order.cart.empty();
  }
}
