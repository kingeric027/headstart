import { CurrentOrderService } from '../current-order/current-order.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { CartService } from '../cart/cart.service';
import { Injectable } from '@angular/core';
import { IShopperContext } from '@app-buyer/ocm-default-components/shopper-context';
import { RouteService } from '../route/route.service';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class ShopperContextService implements IShopperContext {
  constructor(
    public currentOrder: CurrentOrderService,
    public currentUser: CurrentUserService,
    public cartActions: CartService,
    public routeActions: RouteService,
    public productFilterActions: ProductFilterService,
    public authentication: AuthService
  ) {}
}
