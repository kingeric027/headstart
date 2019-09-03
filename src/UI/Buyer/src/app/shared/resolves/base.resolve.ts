import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { CurrentOrderService } from '@app-buyer/shared/services/current-order/current-order.service';
import { CurrentUserService } from '../services/current-user/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class BaseResolve implements Resolve<any> {
  constructor(private currentOrder: CurrentOrderService, private currentUser: CurrentUserService) {}

  resolve() {
    this.currentUser.reset();
    this.currentOrder.reset();
  }
}
