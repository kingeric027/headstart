import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Order, OcOrderService } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SellerUserService } from '@app-seller/shared/services/seller-user/seller-user.service';
import { FormControl, FormGroup } from '@angular/forms';
import { OrderService } from '@app-seller/shared/services/order/order.service';

function createOrderForm(order: Order) {
  return new FormGroup({
    Comments: new FormControl(order.Comments),
  });
}

@Component({
  selector: 'app-order-table',
  templateUrl: './order-table.component.html',
  styleUrls: ['./order-table.component.scss'],
})
export class OrderTableComponent extends ResourceCrudComponent<Order> {
  constructor(
    private orderService: OrderService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, orderService, router, activatedroute, ngZone, createOrderForm);
  }
}
