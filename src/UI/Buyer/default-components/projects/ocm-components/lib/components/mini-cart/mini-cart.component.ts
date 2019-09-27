import { Component, Output, EventEmitter, OnChanges } from '@angular/core';
import { Order, LineItem } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from 'src/app/ocm-default-components/shopper-context';

@Component({
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart extends OCMComponent implements OnChanges {
  @Output() navigate = new EventEmitter(); // to do, use context on pathChange instead?
  lineItems: LineItem[] = [];
  order: Order = {};
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  ngOnChanges() {
    this.order = this.context.currentOrder.order;
    this.lineItems = this.context.currentOrder.lineItems.Items;
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
