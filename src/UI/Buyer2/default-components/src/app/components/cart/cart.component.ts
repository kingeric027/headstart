import { Component, Input, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import { QuantityLimits } from '../../models/quantity-limits';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart extends OCMComponent {
  @Input() quantityLimits: QuantityLimits[];
  @Input() order: Order;
  @Input() lineItems: ListLineItem;

  ngOnContextSet() {}

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
