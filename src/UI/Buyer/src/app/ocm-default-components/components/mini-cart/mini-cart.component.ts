import { Component, Output, EventEmitter, OnChanges, Input } from '@angular/core';
import { Order, LineItem } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { OCMComponent } from '@app-buyer/ocm-default-components/shopper-context';

@Component({
  templateUrl: './mini-cart.component.html',
})
export class OCMMiniCart extends OCMComponent implements OnChanges {
  @Output() navigate = new EventEmitter();
  @Input() lineItems: LineItem[] = [];
  order: Order = {};
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  ngOnChanges() {
    this.context.currentOrder.onOrderChange((order) => (this.order = order));
    this.context.currentOrder.onLineItemsChange((li) => (this.lineItems = li.Items));
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
