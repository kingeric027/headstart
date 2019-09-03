import { Component, Input, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { OCMComponent } from '../ocm-component';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart extends OCMComponent implements OnInit {
  @Input() order: Order;
  @Input() lineItems: ListLineItem;
  @Input() quantityLimits: QuantityLimits[];

  ngOnInit() {}

  log(object: any) {
    console.log(object);
    console.log(this.lineItems);
  }

  toProductList() {
    this.navigator.toProductList();
  }

  toCheckout() {
    this.navigator.toCheckout();
  }

  emptyCart() {
    this.cartActions.emptyCart();
  }
}
