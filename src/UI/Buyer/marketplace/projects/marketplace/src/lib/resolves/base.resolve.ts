import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { ProductCategoriesService } from '../services/product-categories/product-categories.service';
import { CurrentOrderService } from '../services/order/order.service';
import { OrdersToApproveStateService } from '../services/order-history/order-to-approve-state.service';

@Injectable({
  providedIn: 'root',
})
export class BaseResolve implements Resolve<any> {
  constructor(
    private currentOrder: CurrentOrderService,
    private currentUser: CurrentUserService,
    private ordersToApprove: OrdersToApproveStateService,
    private productCategories: ProductCategoriesService
  ) {}

  async resolve() {
    const user = this.currentUser.reset();
    const order = this.currentOrder.reset();
    const ordersToApprove = this.ordersToApprove.reset();
    const categories = this.productCategories.setCategories();
    await Promise.all([user, order, ordersToApprove, categories]);
  }
}
