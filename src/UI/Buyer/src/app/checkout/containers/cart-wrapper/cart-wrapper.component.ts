import { Component, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { CartService, BuildQtyLimits, CurrentOrderService } from '@app-buyer/shared';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'cart-wrapper',
  templateUrl: './cart-wrapper.component.html',
  styleUrls: ['./cart-wrapper.component.scss'],
})
export class CartWrapperComponent implements OnInit {
  order: Order;
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];
  alive = true;

  constructor(
    private cartService: CartService,
    private currentOrder: CurrentOrderService,
    public context: ShopperContextService //used in template
  ) {}

  ngOnInit() {
    this.currentOrder.onOrderChange(this.setOrder);
    this.currentOrder.onLineItemsChange(this.setLineItems);
  }

  setOrder = (order: Order): void => {
    this.order = order;
  };

  setLineItems = (items: ListLineItem): void => {
    this.lineItems = this.cartService.addSpecsToProductName(items);
    this.quantityLimits = this.lineItems.Items.map((li) => BuildQtyLimits(li.Product));
  };
}
