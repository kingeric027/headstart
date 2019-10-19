import { Component, Output, EventEmitter, OnChanges } from '@angular/core';
import { Order, LineItem } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart extends OCMComponent {
  @Output() navigate = new EventEmitter(); // to do, use context on pathChange instead?
  lineItems: LineItem[] = [];
  order: Order = {};
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  ngOnContextSet() {
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
