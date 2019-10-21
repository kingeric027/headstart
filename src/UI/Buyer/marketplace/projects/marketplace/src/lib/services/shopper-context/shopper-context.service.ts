import { CurrentOrderService } from '../current-order/current-order.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { Injectable, Inject } from '@angular/core';
import { RouteService } from '../route/route.service';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { AuthService } from '../auth/auth.service';
import { OcMeService } from '@ordercloud/angular-sdk';
import { OrderHistoryService } from '../order-history/order-history.service';
import { AuthNetCreditCardService } from '../authorize-net/authorize-net.service';
import { applicationConfiguration } from '../../config/app.config';
import { IShopperContext, AppConfig } from '../../shopper-context';

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
    public creditCards: AuthNetCreditCardService,
    @Inject(applicationConfiguration) public appSettings: AppConfig
  ) {}
}
