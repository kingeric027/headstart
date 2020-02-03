import { Component, Input } from '@angular/core';
import { ListLineItemWithProduct, MarketplaceOrder, ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart {
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItemWithProduct;

  constructor(private context: ShopperContextService) { }

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
