import { Component } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  template: `
    <ocm-order-details [context]="context" [orderID]="orderID"></ocm-order-details>
  `,
})
export class OrderDetailWrapperComponent {
  orderID: string;

  constructor(public context: ShopperContextService, private activatedRoute: ActivatedRoute) {
    this.orderID = this.activatedRoute.snapshot.params.orderID;
  }
}
