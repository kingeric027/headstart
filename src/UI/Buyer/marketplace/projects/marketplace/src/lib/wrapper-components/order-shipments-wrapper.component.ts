import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { OrderHistoryService } from '../services/order-history/order-history.service';

@Component({
  template: `
    <ocm-order-shipments [context]="context"></ocm-order-shipments>
  `,
})
export class OrderShipmentsWrapperComponent {
  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute, private orderHistory: OrderHistoryService) {
    this.orderHistory.activeOrderID = this.activatedRoute.snapshot.params.orderID;
  }
}
