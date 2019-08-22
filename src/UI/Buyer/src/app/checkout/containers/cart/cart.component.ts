import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'ocm-shopping-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCartComponent implements OnInit {
  @Input() order: Order;
  @Input() lineItems: ListLineItem;
  @Input() quantityLimits: QuantityLimits[];
  @Output() emptyCart = new EventEmitter<void>();
  @Output() deleteLineItem = new EventEmitter<{ lineItemID: string }>();
  @Output() updateQuantity = new EventEmitter<{ lineItemID: string; quantity: number }>();
  @Output() navigateToProductDetails = new EventEmitter<{ productID: string }>();

  ngOnInit() {}
}
