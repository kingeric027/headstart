// angular
import { Injectable } from '@angular/core';
import { AppConfig } from '../../shopper-context';
import { OrderStateService } from './order-state.service';
import { CartService } from './cart.service';
import { CheckoutService } from './checkout.service';
import { OcLineItemService, OcOrderService, Order } from '@ordercloud/angular-sdk';
import { OrderCloudSandboxService } from '../ordercloud-sandbox/ordercloud-sandbox.service';
import { MarketplaceOrder, MarketplaceLineItem } from 'marketplace-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService {
  onChange = this.state.onOrderChange.bind(this.state);
  reset = this.state.reset.bind(this.state);

  constructor(
    private cartService: CartService,
    private checkoutService: CheckoutService,
    private state: OrderStateService,
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService,
    private ocSandboxService: OrderCloudSandboxService,
    private appConfig: AppConfig
  ) {}

  get(): MarketplaceOrder {
    return this.state.order;
  }

  get cart(): CartService {
    return this.cartService;
  }

  get checkout(): CheckoutService {
    return this.checkoutService;
  }

  async submitQuoteOrder(orderDetails: Order, lineItem: MarketplaceLineItem): Promise<Order> {
    orderDetails.ID = `${this.appConfig.marketplaceID}{orderIncrementor}`;
    const quoteOrder = await this.ocOrderService.Create('Outgoing', orderDetails).toPromise();
    await this.ocLineItemService.Create('Outgoing', quoteOrder.ID, lineItem).toPromise();
    await this.ocSandboxService.calculateOrder(quoteOrder.ID);
    const submittedQuoteOrder = await this.ocOrderService.Submit('Outgoing', quoteOrder.ID).toPromise();
    return submittedQuoteOrder;
  }
}
