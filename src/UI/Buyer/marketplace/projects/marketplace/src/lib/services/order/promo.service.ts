import { Injectable } from '@angular/core';
import { Orders, LineItems, Me, OrderPromotion } from 'ordercloud-javascript-sdk';
import { Subject } from 'rxjs';
import { OrderStateService } from './order-state.service';
import { isUndefined as _isUndefined } from 'lodash';
import { listAll } from '../../functions/listAll';
import { MarketplaceLineItem, MarketplaceOrder, HeadStartSDK, ListPage } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../temp-sdk/temp-sdk.service';

@Injectable({
  providedIn: 'root',
})
export class PromoService {
  public onAdd = new Subject<OrderPromotion>(); // need to make available as observable
  public onChange = this.state.onPromosChange.bind(this.state);
  private initializingOrder = false;

  constructor(private state: OrderStateService, private tempsdk: TempSdk) {}

  get(): ListPage<OrderPromotion> {
    return this.promos;
  }

  public async removePromo(promoCode: string): Promise<void> {
    this.promos.Items = this.promos.Items.filter(p => p.Code !== promoCode);
    try {
      await Orders.RemovePromotion('Outgoing', this.order.ID, promoCode);
    } finally {
      this.state.reset();
    }
  }

  public async applyPromo(promoCode: string): Promise<OrderPromotion> {
    try {
      const newPromo = await Orders.AddPromotion('Outgoing', this.order?.ID, promoCode);
      this.onAdd.next(newPromo);
      return newPromo;
    } finally {
      await this.state.reset();
    }
  }

  public async applyAutomaticPromos(): Promise<void> {
    try {
      await this.tempsdk.applyAutomaticPromotionsToOrder(this.order.ID)
    } finally{
      await this.state.reset();
    }
  }

  private get order(): MarketplaceOrder {
    return this.state.order;
  }

  private set order(value: MarketplaceOrder) {
    this.state.order = value;
  }

  private get promos(): ListPage<OrderPromotion> {
    return this.state.orderPromos;
  }

  private set promos(value: ListPage<OrderPromotion>) {
    this.state.orderPromos = value;
  }
}
