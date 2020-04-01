import { Injectable } from '@angular/core';
import { RouteService } from '../route/route.service';
import { ProductFilterService } from '../product-filter/product-filter.service';
import { AuthService } from '../auth/auth.service';
import { OrderHistoryService } from '../order-history/order-history.service';
import { AppConfig } from '../../shopper-context';
import { SupplierFilterService } from '../supplier-filter/supplier-filter.service';
import { ProductCategoriesService } from '../product-categories/product-categories.service';
import { CurrentOrderService } from '../order/order.service';
import { AddressService } from '../addresses/address.service';
import { CurrentUserService } from '../current-user/current-user.service';
import { UserManagementService } from '../user-management/user-management.service';

@Injectable({
  providedIn: 'root',
})
export class ShopperContextService {
  constructor(
    public order: CurrentOrderService,
    public currentUser: CurrentUserService,
    public router: RouteService,
    public productFilters: ProductFilterService,
    public supplierFilters: SupplierFilterService,
    public authentication: AuthService,
    public orderHistory: OrderHistoryService,
    public appSettings: AppConfig,
    public categories: ProductCategoriesService,
    public userManagementService: UserManagementService,
    public addresses: AddressService
  ) {}
}
