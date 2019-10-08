import { CurrentOrderService } from '../current-order/current-order.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { Injectable, Inject } from '@angular/core';
import { IShopperContext, AppConfig } from 'shopper-context-interface';
import { RouteService } from '../route/route.service';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { AuthService } from '../auth/auth.service';
import { applicationConfiguration } from 'src/app/config/app.config';
import { OcMeService } from '@ordercloud/angular-sdk';
import { OrderHistoryService } from '../order-history/order-history.service';

@Injectable({
  providedIn: 'root',
})
export class ShopperContextService implements IShopperContext {
  constructor(
    public currentOrder: CurrentOrderService,
    public currentUser: CurrentUserService,
    public router: RouteService,
    public productFilters: ProductFilterService,
    public authentication: AuthService,
    public myResources: OcMeService,
    public orderHistory: OrderHistoryService,
    @Inject(applicationConfiguration) public appSettings: AppConfig
  ) {}
}
