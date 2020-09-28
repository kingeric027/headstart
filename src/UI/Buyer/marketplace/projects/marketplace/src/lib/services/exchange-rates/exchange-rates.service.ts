import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ListPage, HeadStartSDK } from '@ordercloud/headstart-sdk';
import { ExchangeRates } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class ExchangeRatesService {
  private ratesSubject: BehaviorSubject<ListPage<ExchangeRates>> = new BehaviorSubject<ListPage<ExchangeRates>>(null);

  constructor(private currentUser: CurrentUserService) {}

  Get(): ListPage<ExchangeRates> {
    return this.exchangeRates;
  }

  async reset(): Promise<void> {
    const me = this.currentUser.get();
    this.exchangeRates = await HeadStartSDK.ExchangeRates.Get(me.Currency);
  }

  private get exchangeRates(): ListPage<ExchangeRates> {
    return this.ratesSubject.value;
  }

  private set exchangeRates(value: ListPage<ExchangeRates>) {
    this.ratesSubject.next(value);
  }
}
