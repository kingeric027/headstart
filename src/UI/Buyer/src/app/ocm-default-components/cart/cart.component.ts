import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { Navigator } from '@app-buyer/shared/services/navigator/navigator.service';

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart implements OnInit {
  @Input() order: Order;
  @Input() lineItems: ListLineItem;
  @Input() quantityLimits: QuantityLimits[];
  @Input() navigator: Navigator;
  @Output() emptyCart = new EventEmitter<void>();
  @Output() deleteLineItem = new EventEmitter<{ lineItemID: string }>();
  @Output() updateQuantity = new EventEmitter<{ lineItemID: string; quantity: number }>();

  ngOnInit() {}

  log(object: any) {
    console.log(object);
    console.log(this.lineItems);
  }
}
