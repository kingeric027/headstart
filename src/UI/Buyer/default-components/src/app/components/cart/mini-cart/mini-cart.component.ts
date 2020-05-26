import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { ShopperContextService, MarketplaceOrder, MarketplaceLineItem } from 'marketplace';

@Component({
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart implements OnInit {
  @Output() navigate = new EventEmitter(); // to do, use context on pathChange instead?
  lineItems: MarketplaceLineItem[] = [];
  order: MarketplaceOrder = {};
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;
  _orderCurrency: string;

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.order = this.context.order.get();
    this.lineItems = this.context.order.cart.get().Items;
    const currentUser = this.context.currentUser.get();
    // Using `|| "USD"` for fallback right now in case there's bad data without the xp value.
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === "BuyerLocation")[0].xp?.Currency || "USD";
  }

  toFullCart(): void {
    this.context.router.toCart();
    this.navigate.emit();
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
    this.navigate.emit();
  }

  toCheckout(): void {
    this.context.router.toCheckout();
    this.navigate.emit();
  }
}
