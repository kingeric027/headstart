import { CurrentOrderService } from '../current-order/current-order.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { CartService } from '../cart/cart.service';
import { Injectable, Inject } from '@angular/core';
import { IShopperContext, AppConfig } from 'shopper-context-interface';
import { RouteService } from '../route/route.service';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { AuthService } from '../auth/auth.service';
import { applicationConfiguration } from 'src/app/config/app.config';
import { OcMeService } from '@ordercloud/angular-sdk';

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
    public authentication: AuthService,
    public myResources: OcMeService,
    @Inject(applicationConfiguration) public appSettings: AppConfig
  ) {}
}
