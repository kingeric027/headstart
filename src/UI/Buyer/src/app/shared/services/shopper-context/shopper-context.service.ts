import { CurrentOrderService } from '../current-order/current-order.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { CartService } from '../cart/cart.service';
import { Injectable } from '@angular/core';
import { ShopperContext } from '@app-buyer/ocm-default-components/shopper-context';
import { RouteService } from '../route/route.service';

@Injectable({
  providedIn: 'root',
})
export class ShopperContextService implements ShopperContext {
  constructor(
    public currentOrder: CurrentOrderService,
    public currentUser: CurrentUserService,
    public cartActions: CartService,
    public routeActions: RouteService
  ) {}
}
