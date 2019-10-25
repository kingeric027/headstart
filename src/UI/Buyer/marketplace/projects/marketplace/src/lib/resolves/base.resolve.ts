import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { CurrentOrderService } from '../services/current-order/current-order.service';

@Injectable({
  providedIn: 'root',
})
export class BaseResolve implements Resolve<any> {
  constructor(private currentOrder: CurrentOrderService, private currentUser: CurrentUserService) {}

  async resolve() {
    const user = this.currentUser.reset();
    const order = this.currentOrder.reset();
    await Promise.all([user, order]);
  }
}
