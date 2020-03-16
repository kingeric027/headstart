// angular
import { Injectable } from '@angular/core';
import { MarketplaceOrder, AppConfig } from '../../shopper-context';
import { OrderStateService } from './order-state.service';
import { CartService, ICart } from './cart.service';
import { CheckoutService, ICheckout } from './checkout.service';
import { OcLineItemService, OcOrderService, Order, LineItem } from '@ordercloud/angular-sdk';

export interface ICurrentOrder {
  cart: ICart;
  checkout: ICheckout;
  get(): MarketplaceOrder;
  submitQuoteOrder(orderDetails: Order, lineItem: LineItem): Promise<Order>;
  onChange(callback: (order: MarketplaceOrder) => void): void;
  reset(): Promise<void>;
}

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {
  onChange = this.state.onOrderChange.bind(this.state);
  reset = this.state.reset.bind(this.state);

  constructor(
    public cart: CartService,
    public checkout: CheckoutService,
    private state: OrderStateService,
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService,
    private appConfig: AppConfig
  ) {}

  get(): MarketplaceOrder {
    return this.state.order;
  }
  async submitQuoteOrder(orderDetails: Order, lineItem: LineItem): Promise<Order> {
    orderDetails.ID = `${this.appConfig.marketplaceID}{orderIncrementor}`;
    const quoteOrder = await this.ocOrderService.Create('Outgoing', orderDetails).toPromise();
    await this.ocLineItemService.Create('Outgoing', quoteOrder.ID, lineItem).toPromise();
    const submittedQuoteOrder = await this.ocOrderService.Submit('Outgoing', quoteOrder.ID).toPromise();
    return submittedQuoteOrder;
  }
}
