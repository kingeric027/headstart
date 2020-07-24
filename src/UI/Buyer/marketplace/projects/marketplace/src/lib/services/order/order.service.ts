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
    const order = {
      ID: `${this.appConfig.marketplaceID}{orderIncrementor}`,
      xp: {
        AvalaraTaxTransactionCode: '',
        OrderType: OrderType.Quote,
        QuoteOrderInfo: {
          FirstName: info.FirstName,
          LastName: info.LastName,
          Phone: info.Phone,
          Email: info.Email,
          Comments: info.Comments,
        },
      },
    };
    const quoteOrder = await Orders.Create('Outgoing', order);
    await LineItems.Create('Outgoing', quoteOrder.ID, lineItem as LineItem);
    await IntegrationEvents.Calculate('Outgoing', quoteOrder.ID);
    const submittedQuoteOrder = await Orders.Submit('Outgoing', quoteOrder.ID);
    return submittedQuoteOrder;
  }
}
