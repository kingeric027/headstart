import { Component, Input, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { ListLineItemWithProduct, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart {
  @Input() order: Order;
  @Input() lineItems: ListLineItemWithProduct;

  constructor(private context: ShopperContextService) {}

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
