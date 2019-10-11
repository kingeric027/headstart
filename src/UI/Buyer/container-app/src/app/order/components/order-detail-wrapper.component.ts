import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ActivatedRoute } from '@angular/router';
import { OrderHistoryService } from 'src/app/shared/services/order-history/order-history.service';

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
