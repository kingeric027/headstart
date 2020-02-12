import { CurrentOrderService, ICurrentOrder } from '../current-order/current-order.service';
import { CurrentUserService, ICurrentUser } from '../current-user/current-user.service';
import { Injectable } from '@angular/core';
import { RouteService, IRouter } from '../route/route.service';
import { ProductFilterService, IProductFilters } from '../product-filter/product-filter.service';
import { AuthService, IAuthentication } from '../auth/auth.service';
import { OrderHistoryService, IOrderHistory } from '../order-history/order-history.service';
import { AppConfig } from '../../shopper-context';
import { SupplierFilterService, ISupplierFilters } from '../supplier-filter/supplier-filter.service';
import { ProductCategoriesService, ICategories } from '../product-categories/product-categories.service';

export interface IShopperContext {
  router: IRouter;
  currentUser: ICurrentUser;
  currentOrder: ICurrentOrder;
  productFilters: IProductFilters;
  categories: ICategories;
  supplierFilters: ISupplierFilters;
  authentication: IAuthentication;
  orderHistory: IOrderHistory;
  appSettings: AppConfig; // TODO - should this come from custom-components repo somehow? Or be configured in admin and persisted in db?
}

@Injectable({
  providedIn: 'root',
})
export class ShopperContextService implements IShopperContext {
  constructor(
    public currentOrder: CurrentOrderService,
    public currentUser: CurrentUserService,
    public router: RouteService,
    public productFilters: ProductFilterService,
    public supplierFilters: SupplierFilterService,
    public authentication: AuthService,
    public orderHistory: OrderHistoryService,
    public appSettings: AppConfig,
    public categories: ProductCategoriesService
  ) {}
}
