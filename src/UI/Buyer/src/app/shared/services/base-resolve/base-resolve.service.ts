// angular
import { Injectable, Inject } from '@angular/core';

// third party
import { OcMeService, Order, OcOrderService, OcTokenService, ListLineItem } from '@ordercloud/angular-sdk';
import { CartService } from '@app-buyer/shared/services/cart/cart.service';
import { AppStateService } from '@app-buyer/shared/services/app-state/app-state.service';
import * as jwtDecode from 'jwt-decode';
import { isUndefined as _isUndefined } from 'lodash';

// app
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';

@Injectable({
  providedIn: 'root',
})
export class BaseResolveService {
  constructor(
    private appStateService: AppStateService,
    private ocMeService: OcMeService,
    private cartService: CartService,
    private ocOrderService: OcOrderService,
    private ocTokenService: OcTokenService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  // Used by BaseResolve when app first loads, at login and at logout
  // auth guards have confirmed at this point a token exists
  async setAppState(): Promise<any> {
    const isAnon = !_isUndefined(this.getOrderIDFromToken());
    this.appStateService.isAnonSubject.next(isAnon);
    const prevLineItems = this.appStateService.lineItemSubject.value.Items.map((li) => {
      return { ProductID: li.xp.product, Quantity: li.Quantity };
    });
    const transferCart: boolean =
      !isAnon && // user is now logged in
      this.appStateService.isAnonSubject.value && // previously, user was anonymous
      prevLineItems.length > 0; // previously, user added to cart
    await Promise.all([this.setCurrentUser(), this.setCurrentOrder()]);
    if (transferCart) {
      await this.cartService.addManyToCart(prevLineItems);
    }
  }

  async setCurrentOrder(): Promise<void> {
    let order: Order;
    const orders = await this.ocMeService.ListOrders({ sortBy: '!DateCreated', filters: { status: 'Unsubmitted' } }).toPromise();
    if (orders.Items.length) {
      order = orders.Items[0];
    }
    if (!order && this.appConfig.anonymousShoppingEnabled) {
      order = <Order>{ ID: this.getOrderIDFromToken() };
    }
    if (!order) {
      order = await this.ocOrderService.Create('outgoing', {}).toPromise();
    }
    this.appStateService.orderSubject.next(order);
    this.setCurrentCart(order);
  }

  private async setCurrentUser(): Promise<void> {
    const user = await this.ocMeService.Get().toPromise();
    this.appStateService.userSubject.next(user);
  }

  private async setCurrentCart(order: Order): Promise<void> {
    let items: ListLineItem = {
      Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
      Items: [],
    };
    if (order.DateCreated) {
      items = await this.cartService.listAllItems(order.ID);
    }
    this.appStateService.lineItemSubject.next(items);
  }

  private getOrderIDFromToken(): string | void {
    return jwtDecode(this.ocTokenService.GetAccess()).orderid;
  }
}
