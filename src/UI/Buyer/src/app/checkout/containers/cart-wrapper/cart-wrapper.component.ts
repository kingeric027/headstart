import { Component, OnInit, OnDestroy } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { CartService, BuildQtyLimits, CurrentOrderService } from '@app-buyer/shared';
import { takeWhile } from 'rxjs/operators';
import { NavigatorService } from '@app-buyer/shared/services/navigator/navigator.service';

@Component({
  selector: 'cart-wrapper',
  templateUrl: './cart-wrapper.component.html',
  styleUrls: ['./cart-wrapper.component.scss'],
})
export class CartWrapperComponent implements OnInit, OnDestroy {
  order: Order;
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];
  alive = true;

  constructor(
    private cartService: CartService,
    private currentOrder: CurrentOrderService,
    protected navigator: NavigatorService //used in template
  ) {}

  ngOnInit() {
    this.currentOrder.orderSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setOrder);
    this.currentOrder.lineItemSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setLineItems);
  }

  setOrder = (order: Order): void => {
    this.order = order;
  };

  setLineItems = (items: ListLineItem): void => {
    this.lineItems = this.cartService.addSpecsToProductName(items);
    this.quantityLimits = this.lineItems.Items.map((li) => BuildQtyLimits(li.Product));
  };

  ngOnDestroy() {
    this.alive = false;
  }
}
