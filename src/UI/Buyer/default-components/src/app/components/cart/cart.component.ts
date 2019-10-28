import { Component, Input, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import { ListLineItemWithProduct } from 'marketplace';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart extends OCMComponent {
  @Input() order: Order;
  @Input() lineItems: ListLineItemWithProduct;

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
