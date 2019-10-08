import { Component, OnInit } from '@angular/core';
import { Order, ListLineItem } from '@ordercloud/angular-sdk';
import { QuantityLimits } from 'src/app/shared/models/quantity-limits';
import { BuildQtyLimits, CurrentOrderService } from 'src/app/shared';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  template: `
    <ocm-cart [order]="order" [lineItems]="lineItems" [context]="context" [quantityLimits]="quantityLimits"></ocm-cart>
  `,
})
export class CartWrapperComponent implements OnInit {
  order: Order;
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];
  alive = true;

  constructor(
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
    this.lineItems = items;
    this.quantityLimits = this.lineItems.Items.map((li) => BuildQtyLimits(li.Product));
  };
}
