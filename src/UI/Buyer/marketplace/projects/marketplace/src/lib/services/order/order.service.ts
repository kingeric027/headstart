// angular
import { Injectable } from '@angular/core';
import { MarketplaceOrder } from '../../shopper-context';
import { OrderStateService } from './order-state.service';
import { CartService, ICart } from './cart.service';
import { CheckoutService, ICheckout } from './checkout.service';

export interface ICurrentOrder {
  cart: ICart;
  checkout: ICheckout;
  get(): MarketplaceOrder;
  onChange(callback: (order: MarketplaceOrder) => void): void;
  reset(): Promise<void>;
}

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {
  onChange = this.state.onOrderChange;
  reset = this.state.reset;

  constructor(public cart: CartService, public checkout: CheckoutService, private state: OrderStateService) {}

  get(): MarketplaceOrder {
    return this.state.order;
  }
}
