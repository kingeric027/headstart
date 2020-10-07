// angular
import { Injectable } from '@angular/core';
import { AppConfig, OrderType } from '../../shopper-context';
import { OrderStateService } from './order-state.service';
import { CartService } from './cart.service';
import { CheckoutService } from './checkout.service';
import { LineItems, Orders, Order, LineItem, IntegrationEvents, Tokens } from 'ordercloud-javascript-sdk';
import { MarketplaceOrder, MarketplaceLineItem, QuoteOrderInfo } from '@ordercloud/headstart-sdk';
import { PromoService } from './promo.service';

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService {
  onChange = this.state.onOrderChange.bind(this.state);
  reset = this.state.reset.bind(this.state);

  constructor(
    private cartService: CartService,
    private promoService: PromoService,
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

  get promos(): PromoService {
    return this.promoService;
  }

  get checkout(): CheckoutService {
    return this.checkoutService;
  }

  async submitQuoteOrder(info: QuoteOrderInfo, lineItem: MarketplaceLineItem): Promise<Order> {
    const order = this.buildQuoteOrder(info);
    lineItem.xp.StatusByQuantity = {
      Submitted: 0,
      Open: 1,
      Backordered: 0,
      Canceled: 0,
      CancelRequested: 0,
      Returned: 0,
      ReturnRequested: 0,
      Complete: 0,
    } as any;
    const quoteOrder = await Orders.Create('Outgoing', order);
    await LineItems.Create('Outgoing', quoteOrder.ID, lineItem as LineItem);
    await IntegrationEvents.Calculate('Outgoing', quoteOrder.ID);
    const submittedQuoteOrder = await Orders.Submit('Outgoing', quoteOrder.ID);
    return submittedQuoteOrder;
  }

  buildQuoteOrder(info: QuoteOrderInfo): Order {
    return {
      ID: `${this.appConfig.marketplaceID}{orderIncrementor}`,
      xp: {
        AvalaraTaxTransactionCode: '',
        OrderType: OrderType.Quote,
        QuoteOrderInfo: {
          FirstName: info.FirstName,
          LastName: info.LastName,
          BuyerLocation: (info as any).BuyerLocation,
          Phone: info.Phone,
          Email: info.Email,
          Comments: info.Comments,
        },
      },
    };
  }
}
