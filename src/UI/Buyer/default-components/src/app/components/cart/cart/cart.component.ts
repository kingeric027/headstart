import { Component, Input, OnInit } from '@angular/core';
import { ListLineItemWithProduct, ShopperContextService, MarketplaceOrder, LineItemWithProduct } from 'marketplace';
import { groupBy as _groupBy } from 'lodash';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart {
  @Input() order: MarketplaceOrder;
  @Input() set lineItems(value: ListLineItemWithProduct) {
    this.liGroupedByShipFrom = this.groupLineItemsByShipFrom(value);
  }

  liGroupedByShipFrom: LineItemWithProduct[][];

  constructor(private context: ShopperContextService) {}

  groupLineItemsByShipFrom(lis: ListLineItemWithProduct) {
    return Object.values(_groupBy(lis.Items, li => li.ShipFromAddressID));
  }

  toProductList() {
    this.context.router.toProductList();
  }

  toCheckout() {
    this.context.router.toCheckout();
  }

  emptyCart() {
    this.context.currentOrder.emptyCart();
  }
}
