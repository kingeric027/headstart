import { Component, Input, OnInit } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ListLineItem, Order } from '@ordercloud/angular-sdk';
import { faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { CartService } from '@app-buyer/shared/services/cart/cart.service';
import { CurrentOrderService } from '@app-buyer/shared/services/current-order/current-order.service';

@Component({
  selector: 'checkout-mini-cart',
  templateUrl: './mini-cart.component.html',
})
export class MiniCartComponent implements OnInit {
  @Input() popover: NgbPopover;
  lineItems: ListLineItem;
  order: Order;
  maxLines = 5; // Limit the height for UI purposes
  faEllipsisH = faEllipsisH;

  constructor(
    private currentOrder: CurrentOrderService,
    protected cartService: CartService // used in template
  ) {}

  ngOnInit() {
    this.lineItems = this.currentOrder.lineItems;
    this.order = this.currentOrder.order;
    this.lineItems = this.cartService.addSpecsToProductName(this.lineItems);
  }

  close() {
    if (this.popover.isOpen()) {
      this.popover.close();
    }
  }
}
