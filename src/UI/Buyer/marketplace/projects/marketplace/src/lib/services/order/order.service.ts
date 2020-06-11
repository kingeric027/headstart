// angular
import { Injectable } from '@angular/core';
import { AppConfig } from '../../shopper-context';
import { OrderStateService } from './order-state.service';
import { CartService } from './cart.service';
import { CheckoutService } from './checkout.service';
import { LineItems, Orders, Order, LineItem, IntegrationEvents } from 'ordercloud-javascript-sdk';
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
    const quoteOrder = await Orders.Create('Outgoing', orderDetails);
    await LineItems.Create('Outgoing', quoteOrder.ID, lineItem as LineItem);
    await IntegrationEvents.Calculate('Outgoing', quoteOrder.ID);
    const submittedQuoteOrder = await Orders.Submit('Outgoing', quoteOrder.ID);
    return submittedQuoteOrder;
  }
}
