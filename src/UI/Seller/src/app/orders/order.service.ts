import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Order, OcOrderService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { OrderType } from '@app-seller/shared/models/MarketPlaceOrder.interface';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class OrderService extends ResourceCrudService<Order> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocOrderService: OcOrderService, currentUserService: CurrentUserService) {
    super(router, activatedRoute, ocOrderService, currentUserService, '/orders', 'orders');
  }
  setOrderDirection(orderDirection: string) {
    this.patchFilterState({ OrderDirection: orderDirection });
  }
  isQuoteOrder(order: Order) {
    return order?.xp?.OrderType === OrderType.Quote;
  }
}
