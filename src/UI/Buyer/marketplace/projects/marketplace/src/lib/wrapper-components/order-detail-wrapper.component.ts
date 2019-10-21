import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { OrderHistoryService } from '../services/order-history/order-history.service';

@Component({
  template: `
    <ocm-order-details [context]="context"></ocm-order-details>
  `,
})
export class OrderDetailWrapperComponent {
  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute, private orderHistory: OrderHistoryService) {
    this.orderHistory.activeOrderID = this.activatedRoute.snapshot.params.orderID;
  }
}
