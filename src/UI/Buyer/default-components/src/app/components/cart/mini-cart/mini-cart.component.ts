import { Component, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { ShopperContextService, MarketplaceOrder } from 'marketplace';

@Component({
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart implements OnInit {
  @Output() navigate = new EventEmitter(); // to do, use context on pathChange instead?
  lineItems: LineItem[] = [];
  order: MarketplaceOrder = {};
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.order = this.context.currentOrder.get();
    this.lineItems = this.context.currentOrder.getLineItems().Items;
  }

  toFullCart() {
    this.context.router.toCart();
    this.navigate.emit();
  }

  toProductDetails(productID: string) {
    this.context.router.toProductDetails(productID);
    this.navigate.emit();
  }

  toCheckout() {
    this.context.router.toCheckout();
    this.navigate.emit();
  }
}
