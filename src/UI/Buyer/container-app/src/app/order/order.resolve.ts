import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { OrderHistoryService } from '../shared/services/order-history/order-history.service';

@Injectable({
  providedIn: 'root',
})
export class OrderResolve implements Resolve<any> {
  constructor(private orderHistory: OrderHistoryService) {}

  resolve(route: ActivatedRouteSnapshot) {
    const orderID = route.paramMap.get('orderID');
    return this.orderHistory.getOrderDetails(orderID);
  }
}
